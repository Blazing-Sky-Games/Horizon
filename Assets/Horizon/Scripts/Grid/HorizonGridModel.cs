using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Gamelogic.Grids;

public class HorizonGridModel : GridBehaviour<RectPoint> 
{
//private feilds

//public feilds

	public List<HorizonUnitGameView> TurnOrder = new List<HorizonUnitGameView>();
	private int activeIndex = 0;

	public HorizonUnitModel[] FriendlyUnits;
	public HorizonUnitModel[] EnemyUnits;

	public event Action<HorizonGridModel> OnWin;
	public event Action<HorizonGridModel> OnLoss;

//public properties, private backing feilds, and changed events

	//cell view grid, gives accsess to cell views by rect point
	private IGrid<HorizonCellGameView, RectPoint> cellViewGrid;

	public IGrid<HorizonCellGameView, RectPoint> CellViewGrid
	{
		get
		{
			if(cellViewGrid == null)
			{
				cellViewGrid = Grid.CastValues<HorizonCellGameView, RectPoint>();
			}
			
			return cellViewGrid;
		}
	}

	// cell properties
	private HorizonCellGameView selectedCell;
	private HorizonCellGameView highlightedCell;

	public event Action<HorizonCellGameView> OnCellSelected;
	public event Action<HorizonCellGameView> OnHighlightedCellChanged;

	public HorizonCellGameView SelectedCell
	{
		get
		{
			return selectedCell;
		}
		set
		{
			if(OnCellSelected != null)
				OnCellSelected(value);

			selectedCell = value;
		}
	}
	
	public HorizonCellGameView HighlightedCell
	{
		get
		{
			return highlightedCell;
		}
		set
		{
			if(value != highlightedCell && OnHighlightedCellChanged != null)
				OnHighlightedCellChanged(value);
			
			highlightedCell = value;
		}
	}

	//unit properties
	private HorizonUnitModel selectedUnit;
	private HorizonUnitModel highlightedUnit;
	private HorizonUnitModel activeUnit;

	public event Func<HorizonUnitModel,bool> OnUnitSelected;
	public event Action<HorizonUnitModel> OnHighlightedUnitChanged;
	public event Action<HorizonUnitModel> OnActiveUnitChanged;


	public HorizonUnitModel SelectedUnit
	{
		get
		{
			return selectedUnit;
		}
		set
		{
			bool shouldchangeSelection = false;
			if(OnUnitSelected != null) shouldchangeSelection = OnUnitSelected(value);

			if(shouldchangeSelection)
				selectedUnit = value;
		}
	}
	public HorizonUnitModel HighlightedUnit
	{
		get
		{
			return highlightedUnit;
		}
		set
		{
			if(highlightedUnit != value && OnHighlightedUnitChanged != null)
				OnHighlightedUnitChanged(value);

			highlightedUnit = value;
		}
	}

	void HandleOnTurnFinished ()
	{
		IncrementTurnOrder();
	}

	public HorizonUnitModel ActiveUnit
	{
		get
		{
			return activeUnit;	
		}
		private set
		{
			if(activeUnit != value && OnActiveUnitChanged != null)
				OnActiveUnitChanged(value);

			if(activeUnit != null) activeUnit.OnTurnFinished -= HandleOnTurnFinished;

			if(value != null) value.OnTurnFinished += HandleOnTurnFinished;

			activeUnit = value;
			SelectedUnit = activeUnit;
		}
	}

//private callbacks

//private methods
	
//public callbacks

	public override void InitGrid ()
	{
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;

		ActiveUnit = TurnOrder[activeIndex].model;
		CombatUI.Instance.turnOrderDiaplsy.turnOrder = TurnOrder;
		CombatUI.Instance.turnOrderDiaplsy.UpdateTurnOrder();

		SelectedUnit = ActiveUnit;

		foreach(var item in CellViewGrid)
		{
			CellViewGrid[item].model.PositionPoint = item;
		}

		HorizonUnitModel.OnUnitHPEqualsZero += HandleOnUnitHPEqualsZero;
	}

	void IncrementTurnOrder ()
	{
		activeIndex++;
		if(activeIndex >= TurnOrder.Count) activeIndex = 0;

		if(TurnOrder.Count != 0)
		{
			ActiveUnit = TurnOrder[activeIndex].model;
		}
	}

	void HandleOnUnitHPEqualsZero (HorizonUnitModel unit)
	{
		if(unit == activeUnit) IncrementTurnOrder();

		TurnOrder.Remove(unit.view);
		CombatUI.Instance.turnOrderDiaplsy.UpdateTurnOrder();

		if(unit.unitType == UnitType.Character)
		{
			FriendlyUnits = FriendlyUnits.Where(x => x != unit).ToArray();
		}
		else
		{
			EnemyUnits = EnemyUnits.Where(x => x != unit).ToArray();
		}

		Destroy(unit.gameObject);

		CheckWinLoss();
	}

	void CheckWinLoss ()
	{
		if(FriendlyUnits.Count() == 0)
		{
			if(OnLoss != null) 
			{
				OnLoss(this);
			}
		}
		else if(EnemyUnits.Count() == 0)
		{
			if(OnWin != null) 
			{
				OnWin(this);
			}
		}
	}

//public methods

	//get a range of cells given cell movecost and accsesability functions
	//todo: provide custom neighbor checking
	public IEnumerable<RectPoint> GetPointsInRangeCost(
		RectPoint start,
		float moveRange,
		Func<HorizonCellGameView, bool> isAcessible,
		Func<RectPoint, RectPoint, float> getCellMoveCost,
		Func<RectPoint, float, bool> isPointIncluded)
	{
		IGrid<HorizonCellGameView, RectPoint> grid = cellViewGrid;

		//the points on the edge of the search area that are still within range
		HashSet<RectPoint> frontier = new HashSet<RectPoint>();
		
		//the points that have been visited
		HashSet<RectPoint> visited = new HashSet<RectPoint>();
		
		//keep track of the shortest path to each point
		Dictionary<RectPoint, float> costToMoveTo = new Dictionary<RectPoint, float>();

		List<RectPoint> PointsInRange = new List<RectPoint>();
		
		// init
		frontier.Add(start);
		costToMoveTo[start] = 0;
		
		// while there are still cells in the frontier to explore
		while(frontier.Count != 0)
		{
			//keepo track of the new frontier for the next iteration
			HashSet<RectPoint> newFrontier = new HashSet<RectPoint>();
			foreach(RectPoint point in frontier)
			{
				//add the unvisited neighbors in range to the new frontier
				List<RectPoint> unvisitedNeighborsinRange = grid
					.GetAllNeighbors(point)
					.Where(x=> grid.IsOutside(x) == false && isAcessible(grid[x]))
					.Where(x=> visited.Contains(x) == false)
					.Where(x=> costToMoveTo[point] + getCellMoveCost(point,x) <= moveRange)
					.ToList();
				
				newFrontier.UnionWith(unvisitedNeighborsinRange);
				
				// compute the shortest path to the unvisited neighbors
				foreach(RectPoint neighbor in unvisitedNeighborsinRange)
				{
					float totalcostToNeighbor = costToMoveTo[point] + getCellMoveCost(point,neighbor);
					if(costToMoveTo.ContainsKey(neighbor) == false || costToMoveTo[neighbor] > totalcostToNeighbor)
					{
						// we have the first path to reach this point, or a faster path
						costToMoveTo[neighbor] = totalcostToNeighbor;
					}
				}

				PointsInRange.AddRange(newFrontier.Where( (x) =>
				{
					return isPointIncluded(x,costToMoveTo[x]);
				}));
				
				// we have now visited this point
				visited.Add(point);
			}
			
			// explor the new frontier
			frontier = newFrontier;
		}
		
		return PointsInRange;
	}

	public IEnumerable<RectPoint> ShortestPathBetweenPoints(RectPoint start, RectPoint end, Func<HorizonCellGameView,bool> isAccesable)
	{
		IEnumerable<RectPoint> path = Algorithms.AStar(
			cellViewGrid,
			start,
			end,
		   	(p, q) =>p.DistanceFrom(q),
			isAccesable,
			(x,y) => 1
		);

		if(path == null) return null;
		else return path.Skip(1);
	}

	void OnDestroy()
	{
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;
	}
}
