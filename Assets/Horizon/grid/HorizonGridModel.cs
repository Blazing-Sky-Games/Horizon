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

//public properties, private backing feilds, and changed events

	//cell view grid, gives accsess to cell views by rect point
	private IGrid<HorizonCellView, RectPoint> cellViewGrid;

	public IGrid<HorizonCellView, RectPoint> CellViewGrid
	{
		get
		{
			if(cellViewGrid == null)
			{
				cellViewGrid = Grid.CastValues<HorizonCellView, RectPoint>();
			}
			
			return cellViewGrid;
		}
	}

	// cell properties
	private HorizonCellView selectedCell;
	private HorizonCellView highlightedCell;

	public event Action<HorizonCellView> OnCellSelected;
	public event Action<HorizonCellView> OnHighlightedCellChanged;

	public HorizonCellView SelectedCell
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
	
	public HorizonCellView HighlightedCell
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

	public event Action<HorizonUnitModel> OnUnitSelected;
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
			if(OnUnitSelected != null) OnUnitSelected(value);

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

			activeUnit = value;
		}
	}

//private callbacks

//private methods
	
//public callbacks

	public override void InitGrid ()
	{
		foreach(var item in CellViewGrid)
		{
			CellViewGrid[item].model.PositionPoint = item;
		}
	}

//public methods

	//get a range of cells given cell movecost and accsesability functions
	//todo: provide custom neighbor checking
	public IEnumerable<RectPoint> GetPointsInRangeCost(
		RectPoint start,
		float moveRange,
		Func<HorizonCellView, bool> isAcessible,
		Func<RectPoint, RectPoint, float> getCellMoveCost,
		Func<RectPoint, float, bool> isPointIncluded)
	{
		IGrid<HorizonCellView, RectPoint> grid = cellViewGrid;

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

	public IEnumerable<RectPoint> ShortestPathBetweenPoints(RectPoint start, RectPoint end, Func<HorizonCellView,bool> isAccesable)
	{
		return Algorithms.AStar(
			cellViewGrid,
			start,
			end,
		   	(p, q) =>p.DistanceFrom(q),
			isAccesable,
			(x,y) => 1
		).Skip(1);
	}
}
