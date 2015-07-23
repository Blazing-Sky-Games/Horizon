using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gamelogic.Grids;

// ok .... this is one of the bug files
// in this file i experimented with blocking off sections with comments
// hmm ........ after thinking about it, i wonder if it would be a good idea
// to just write our own grid/cell code from scratch
// we are just useing a simple rectagulare grid, and using the Gamelogic.Grids does cause some awkwardness
// hmm ... it would prbably be a lot of work to replace the Gamelogic.Grids stuff with our own grid code ... or maybe not
// that is a project for another week
public class HorizonGridModel : GridBehaviour<RectPoint> 
{
//private feilds

//public feilds

	//the turn order
	public List<HorizonUnitGameView> TurnOrder = new List<HorizonUnitGameView>();
	// whose turn is it in the turn order
	private int activeIndex = 0;

	// which units are characters
	public HorizonUnitModel[] FriendlyUnits;
	// which units are enemys
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
	private HorizonCellGameView selectedCell; // the currently selected cell
	private HorizonCellGameView highlightedCell; // the currently highlighted cell

	// changed events
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
				OnCellSelected(value); // alsways fire the cell selected event

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
				OnHighlightedCellChanged(value); // only fire this event if the value changes
			
			highlightedCell = value;
		}
	}

	//unit properties
	private HorizonUnitModel selectedUnit; // currently selected unit
	private HorizonUnitModel highlightedUnit; // currently highlighted unit
	private HorizonUnitModel activeUnit; // active unit

	// changed events
	public event Func<HorizonUnitModel,bool> OnUnitSelected; // returns a bool to indicate if the value should ovewrite the old value
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

			// this is a hack ... i cant even remember why i needed to do this
			// whe should probably have the consecpt of "onunitclicked" vs "on unit selected" 
			// so you can click a unit without nessisaraly selecting it
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
				OnHighlightedUnitChanged(value); // only fire this event if the value changes

			highlightedUnit = value;
		}
	}

	// event handler for when the active unit finishes its turn
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
				OnActiveUnitChanged(value); // only fire if the value has changed

			if(activeUnit != null) activeUnit.OnTurnFinished -= HandleOnTurnFinished; // remove event handler from previod unit

			if(value != null) value.OnTurnFinished += HandleOnTurnFinished; // and handler to new unit

			activeUnit = value;
			SelectedUnit = activeUnit; // always select the new active unit
		}
	}

//private callbacks

//private methods
	
//public callbacks

	public override void InitGrid ()
	{
		// this is probalb not needed
		// OnUnitHPEqualsZero is a static event in HorizonUnitModel right now
		// this is dangourous and can cause memory leeks (i know right, memory leeks in c#)
		// this is a safegaurd to prevent that
		// really OnUnitHPEqualsZero should just not be static
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;

		// set active unit to correct unit in turn order
		ActiveUnit = TurnOrder[activeIndex].model;

		// init the combatUI
		// hmm .... i dont like amodel talking to a view
		// it should be the other way around
		CombatUI.Instance.turnOrderDiaplsy.turnOrder = TurnOrder;
		CombatUI.Instance.turnOrderDiaplsy.UpdateTurnOrder();

		// select the active unit
		SelectedUnit = ActiveUnit;

		// set the posision point of all cells
		// ugh ... we should really just write our own grid code
		foreach(var item in CellViewGrid)
		{
			CellViewGrid[item].model.PositionPoint = item;
		}

		// add OnUnitHPEqualsZero event handler
		HorizonUnitModel.OnUnitHPEqualsZero += HandleOnUnitHPEqualsZero;
	}

	void IncrementTurnOrder ()
	{
		// move to next unit in order, or go back to start
		activeIndex++;
		if(activeIndex >= TurnOrder.Count) activeIndex = 0;

		//set the new active unit
		if(TurnOrder.Count != 0)
		{
			ActiveUnit = TurnOrder[activeIndex].model;
		}
	}

	void HandleOnUnitHPEqualsZero (HorizonUnitModel unit)
	{
		// if the active unit died increment the turn order ... wait what ... how would this happen?
		if(unit == activeUnit) IncrementTurnOrder();

		// remove the unit
		TurnOrder.Remove(unit.view);
		CombatUI.Instance.turnOrderDiaplsy.UpdateTurnOrder(); // ugh, CombatUI should really just be listening to an UpdateTurnOrder event or somthing

		// update the faction lists
		if(unit.unitType == UnitType.Character)
		{
			FriendlyUnits = FriendlyUnits.Where(x => x != unit).ToArray();
		}
		else
		{
			EnemyUnits = EnemyUnits.Where(x => x != unit).ToArray();
		}

		// begon!
		// later, this needs to wait for death anims and stuff
		// hmm ... can this be backed into the logic in the UnitModel
		Destroy(unit.gameObject);

		// something has died, have we one or lost yet?
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
	// i could have just used the one provided with the library...but i wanted to write it my way
	public IEnumerable<RectPoint> GetPointsInRangeCost(
		RectPoint start, // the cell we start looking from
		float moveRange, // how far can we move
		Func<HorizonCellGameView, bool> isAcessible, // can we walk over this cell
		Func<RectPoint, RectPoint, float> getCellMoveCost, //how expensive is it to move to this cell
		Func<RectPoint, float, bool> isPointIncluded) //can we end at this cell
	{
		//alias for cell view grid .. easyer than typeing cellviewgrid all over the place
		IGrid<HorizonCellGameView, RectPoint> grid = cellViewGrid;

		//the points on the edge of the search area that are still within range
		HashSet<RectPoint> frontier = new HashSet<RectPoint>();
		
		//the points that have been visited
		HashSet<RectPoint> visited = new HashSet<RectPoint>();
		
		//keep track of the shortest cost to move to each point
		Dictionary<RectPoint, float> costToMoveTo = new Dictionary<RectPoint, float>();

		// the cells that we will return
		List<RectPoint> PointsInRange = new List<RectPoint>();
		
		// init, add start to correct sets
		frontier.Add(start);
		costToMoveTo[start] = 0;
		
		// while there are still cells in the frontier to explore
		while(frontier.Count != 0)
		{
			//keep track of the new frontier for the next iteration
			HashSet<RectPoint> newFrontier = new HashSet<RectPoint>();
			foreach(RectPoint point in frontier)
			{
				//add the unvisited neighbors in range to the new frontier
				List<RectPoint> unvisitedNeighborsinRange = grid
					.GetAllNeighbors(point) // the cell is a neighbor
					.Where(x=> grid.IsOutside(x) == false && isAcessible(grid[x])) // the cell is part of the grid and is accsesable
					.Where(x=> visited.Contains(x) == false) // is unvisited
					.Where(x=> costToMoveTo[point] + getCellMoveCost(point,x) <= moveRange) // is within range
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
					//add all points that are valid stoping points
					return isPointIncluded(x,costToMoveTo[x]);
				}));
				
				// we have now visited this point
				visited.Add(point);
			}
			
			// explore the new frontier
			frontier = newFrontier;
		}
		
		// return the range
		return PointsInRange;
	}

	// wrapper around library astar function
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
		else return path.Skip(1); // dont include the starting cell in the path
	}

	void OnDestroy()
	{
		// remove handler from that pesky static event
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;
	}
}
