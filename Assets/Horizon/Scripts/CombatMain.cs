using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gamelogic.Grids;

// oh boy here we go, this is going to be a whopper of a file
// this class controls cell highlighting,
// and the highlevel logic of how the game works
public class CombatMain : MonoBehaviour 
{
	public HorizonGridGameView GridView;
	public HorizonGridModel GridModel;

	private UnitType activeUnitType = UnitType.Character;

	void Start()
	{
		//subscribe to a shitload of events

		GridModel.OnCellSelected += HandleOnCellSelected;
		GridModel.OnHighlightedCellChanged += HandleOnHighlightedCellChanged;

		GridModel.OnUnitSelected += HandleOnUnitSelected;
		GridModel.OnHighlightedUnitChanged += HandleOnHighlightedUnitChanged;
		GridModel.OnActiveUnitChanged += HandleOnActiveUnitChanged;

		GridModel.OnLoss += HandleOnLoss;
		GridModel.OnWin += HandleOnWin;

		// protect our self from that stupid static event
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;
		HorizonUnitModel.OnUnitHPEqualsZero += HandleOnUnitHPEqualsZero;

		CombatUI.Instance.attackButton.OnTargetBegin += HandleOnTargetBegin;
		CombatUI.Instance.attackButton.OnTargetEnd += HandleOnTargetEnd;
	}

	void HandleOnTargetBegin ()
	{
		// highlight the cells in attack range
		GridView.pushHighlightSet(
			GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint),
		    Color.cyan * new Color(1,1,1,0.5f)
		);
	}

	void HandleOnTargetEnd ()
	{
		// unhighlight the cells in attack range
		GridView.popHighlightSet();
	}

	void HandleOnUnitHPEqualsZero (HorizonUnitModel unit)
	{
		// if the unit that died was the selected unit, unselect all uits
		// wait ... could this even happen?
		if(unit == GridModel.SelectedUnit)
			GridModel.SelectedUnit = null;
	}

	// on win
	void HandleOnWin (HorizonGridModel model)
	{
		if(model == GridModel)
			Application.LoadLevel(1);
	}

	// on loss
	void HandleOnLoss (HorizonGridModel model)
	{
		if(model == GridModel)
			Application.LoadLevel(2);
	}

	bool HandleOnUnitSelected (HorizonUnitModel Unit)
	{
		// if we are selecting an attack target
		if(CombatUI.Instance.attackButton.Selecting)
		{
			// if the unit is in range
			if(GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(Unit.PositionPoint))
			{
				// if the cell under the unit is highlighted (because it is in the attack range), remove that highlight
				if(GridModel.HighlightedCell != null && GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(GridModel.HighlightedCell.model.PositionPoint))
					GridView.popHighlightSet();

				// stay on target! stay on target!
				CombatUI.Instance.attackButton.TargetUnit(Unit);
			}

			// do not change the selected unit in the model
			return false;
		}

		// if the mouse is over a cell in the movment range of the previosly selected unit, there will be that green path highlight
		// we need to remove that
		// god cell highlighting is such a mess right now
		if(GridModel.SelectedUnit != null && GridModel.HighlightedCell != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			GridView.popHighlightSet();

		// remove the move range highlight
		GridView.popHighlightSet();

		//if the new unit is an actuall unit
		if(Unit != null)
		{
			// you cant select enemys durng their turn
			if(Unit == GridModel.ActiveUnit && Unit.unitType == UnitType.Enemy)
			{
				GridModel.SelectedUnit = null;
				StartCoroutine(enemyAiRoutine()); // AI baby!
				return false;
			}

			// highlight the movment range
			GridView.pushHighlightSet(Unit.PointsInMovmentRange,Color.red * new Color(1,1,1,0.5f));

			// highlight the shortest path
			if(Unit != null && GridModel.HighlightedCell != null && Unit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			{
				GridView.pushHighlightSet(
					Unit.ShortestPathToPoint(GridModel.HighlightedCell.model.PositionPoint), 
					Color.green * new Color(1,1,1,0.5f)
				);
			}
		}

		// do set the selected unit to the new unit
		return true;
	}

	IEnumerator enemyAiRoutine()
	{
		yield return 0; // wait a frame ... uhh ... why

		// alias for GridModel.ActiveUnit
		// hmm ... should we thros an exception if the unit is not an enemy
		HorizonUnitModel enemy = GridModel.ActiveUnit;

		//attack if we can
		IEnumerable<RectPoint> attackRange = GridModel.CellViewGrid.GetNeighbors(enemy.PositionPoint);
		foreach(RectPoint point in attackRange)
		{
			if(GridModel.CellViewGrid[point].model.OccupyingUnit != null && GridModel.CellViewGrid[point].model.OccupyingUnit.unitType == UnitType.Character)
			{
				//would be replaced by ability selection logic
				yield return new WaitForSeconds(0.5f);
				enemy.Attack(GridModel.CellViewGrid[point].model.OccupyingUnit);
				yield return new WaitForSeconds(0.5f);
				break;
			}
		}

		//move towords the closest character
		List<IEnumerable<RectPoint>> ditancesToUnits = new List<IEnumerable<RectPoint>>();
		foreach(HorizonUnitModel character in GridModel.FriendlyUnits)
		{
			foreach(RectPoint characterNeighbor in GridModel.CellViewGrid.GetNeighbors(character.PositionPoint))
			{
				if( GridModel.CellViewGrid[characterNeighbor].model.OccupyingUnit == null)
				{
					IEnumerable<RectPoint> path = enemy.ShortestPathToPoint(characterNeighbor);
					if(path != null)ditancesToUnits.Add (path);
				}
			}
		}

		if(ditancesToUnits.Count() != 0)
		{
			// hmm ... include information about the unit you are travling towords, and include it in a heuristic measurment
			// this by itself migh tbe good enough to make the ai enjoyable
			IEnumerable<RectPoint> shortestPath = ditancesToUnits.OrderBy(x=>x.Count()).First();
			
			if(shortestPath.Count() <= enemy.speed) 
			{
				if(shortestPath.Count() != 0)
				{
					yield return StartCoroutine(enemy.TraversePath(shortestPath));
				}
			}
			else
			{
				IEnumerable<RectPoint> path = shortestPath.Take(enemy.speed);
				yield return StartCoroutine(enemy.TraversePath(path));
			}
		}

		//attack if we couldnt attack befor
		if(enemy.hasAttacked == false)
		{
			attackRange = GridModel.CellViewGrid.GetNeighbors(enemy.PositionPoint);
			foreach(RectPoint point in attackRange)
			{
				if(GridModel.CellViewGrid[point].model.OccupyingUnit != null && GridModel.CellViewGrid[point].model.OccupyingUnit.unitType == UnitType.Character)
				{
					//would be replaced by ability selection logic
					yield return new WaitForSeconds(0.5f);
					enemy.Attack(GridModel.CellViewGrid[point].model.OccupyingUnit);
					yield return new WaitForSeconds(0.5f);
					break;
				}
			}
		}

		// end ai turn
		enemy.PassTurn();
	}

	//when the active unit changes
	void HandleOnActiveUnitChanged (HorizonUnitModel NewUnit)
	{
		GridModel.HighlightedCell = null; // clear a bunch of highlights
		GridModel.HighlightedUnit = null; 
		GridModel.SelectedCell = null;

		activeUnitType = NewUnit.unitType;
		GridView.handleInput = activeUnitType == UnitType.Character; // dont handleinput if it is the enmys turn
	}

	//when a unit is highlighted/unhighlighted
	void HandleOnHighlightedUnitChanged (HorizonUnitModel NewUnit)
	{
		//update highlight outline
		if(GridModel.HighlightedUnit != null)
		{
			GridModel.HighlightedUnit.gameObject.GetComponent<HorizonUnitGameView>().OutlinePercentage = 0;
		}

		if(NewUnit != null && NewUnit != GridModel.SelectedUnit)
		{
			NewUnit.gameObject.GetComponent<HorizonUnitGameView>().OutlinePercentage = 80;
		}
	}

	// wrapes the unit movment stuff
	void MoveSelectedUnitTo(RectPoint point)
	{
		HorizonUnitModel unit = GridModel.SelectedUnit;

		//unselect the unit to hide some ui
		GridModel.SelectedUnit = null;
		//disable user input
		GridView.handleInput = false;

		Action OnMoveEnd;
		OnMoveEnd = () => 
		{
			unit.OnTraversePathEnd -= OnMoveEnd;
			//enable user input
			GridView.handleInput = true;
			GridModel.SelectedUnit = GridModel.ActiveUnit; // reselect the unit that just moved
		};

		// hook up our event handler
		unit.OnTraversePathEnd += OnMoveEnd;

		//go forth!
		StartCoroutine(unit.TraverseShortestPathToPoint(point));
	}

	//when a cell is clikced
	void HandleOnCellSelected (HorizonCellGameView Cell)
	{
		// if we are selecting an attack target
		if(CombatUI.Instance.attackButton.Selecting)
		{
			if(GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(Cell.model.PositionPoint) 
			   && Cell.model.OccupyingUnit != null && Cell.model.OccupyingUnit.unitType != GridModel.SelectedUnit.unitType)
			{
				GridModel.SelectedUnit = Cell.model.OccupyingUnit;
			}
		}
		// if this is a point in movement range
		else if(Cell != null && GridModel.SelectedUnit != null && GridModel.SelectedUnit == GridModel.ActiveUnit && GridModel.SelectedUnit.PointsInMovmentRange.Contains(Cell.model.PositionPoint))
		{
			//move unit
			MoveSelectedUnitTo(Cell.model.PositionPoint);
		}
	}

	//when a what cell the mouse is over changes
	void HandleOnHighlightedCellChanged (HorizonCellGameView NewCell)
	{
		// if we are selecting an attack target, highligte the cell if it is in attack range
		if(CombatUI.Instance.attackButton.Selecting)
		{
			IEnumerable<RectPoint> pointsInAttackRange = GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint);

			//unhighlight the old cell
			if(GridModel.HighlightedCell != null && pointsInAttackRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
				GridView.popHighlightSet();

			//highlight the new cell
			if(NewCell != null && pointsInAttackRange.Contains(NewCell.model.PositionPoint))
				GridView.pushHighlightSet(new List<RectPoint>{NewCell.model.PositionPoint},Color.cyan * new Color(1,1,1,0.8f));

			return;
		}

		// if the old cell was in the movment range of the active unit, and that unit is selected, we need to pop the old shortest path
		if(GridModel.SelectedUnit != null && GridModel.SelectedUnit == GridModel.ActiveUnit && GridModel.HighlightedCell != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			GridView.popHighlightSet();

		if(NewCell != null)
		{
			// if the new cell is in the movment range of the active unit, and that unit is selected, we need to push the shortest path
			if(GridModel.SelectedUnit != null && GridModel.SelectedUnit == GridModel.ActiveUnit && GridModel.SelectedUnit.PointsInMovmentRange.Contains(NewCell.model.PositionPoint))
			{
				GridView.pushHighlightSet(
					GridModel.SelectedUnit.ShortestPathToPoint(NewCell.model.PositionPoint), 
					Color.green * new Color(1,1,1,0.5f)
				);
			}
		}
	}

	void OnDestroy()
	{
		//unsubscribe from a shitload of events

		GridModel.OnCellSelected -= HandleOnCellSelected;
		GridModel.OnHighlightedCellChanged -= HandleOnHighlightedCellChanged;
		
		GridModel.OnUnitSelected -= HandleOnUnitSelected;
		GridModel.OnHighlightedUnitChanged -= HandleOnHighlightedUnitChanged;
		GridModel.OnActiveUnitChanged -= HandleOnActiveUnitChanged;
		
		GridModel.OnLoss -= HandleOnLoss;
		GridModel.OnWin -= HandleOnWin;
		
		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;
	}
}
