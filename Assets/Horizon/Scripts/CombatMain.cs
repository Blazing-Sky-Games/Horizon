using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Gamelogic.Grids;

public class CombatMain : MonoBehaviour 
{
	public HorizonGridGameView GridView;
	public HorizonGridModel GridModel;

	private UnitType activeUnitType = UnitType.Character;

	void Start()
	{
		GridModel.OnCellSelected += HandleOnCellSelected;
		GridModel.OnHighlightedCellChanged += HandleOnHighlightedCellChanged;

		GridModel.OnUnitSelected += HandleOnUnitSelected;
		GridModel.OnHighlightedUnitChanged += HandleOnHighlightedUnitChanged;
		GridModel.OnActiveUnitChanged += HandleOnActiveUnitChanged;

		GridModel.OnLoss += HandleOnLoss;
		GridModel.OnWin += HandleOnWin;

		HorizonUnitModel.OnUnitHPEqualsZero -= HandleOnUnitHPEqualsZero;
		HorizonUnitModel.OnUnitHPEqualsZero += HandleOnUnitHPEqualsZero;

		CombatUI.Instance.attackButton.OnTargetBegin += HandleOnTargetBegin;
		CombatUI.Instance.attackButton.OnTargetEnd += HandleOnTargetEnd;
	}

	void HandleOnTargetBegin ()
	{
		//if(GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
		//{
			//GridView.popHighlightSet();
		//}

		GridView.pushHighlightSet(
			GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint),
		    Color.cyan * new Color(1,1,1,0.5f)
		);
	}

	void HandleOnTargetEnd ()
	{
		GridView.popHighlightSet();

		//if(GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
		//{
			//GridView.pushHighlightSet(GridModel.SelectedUnit.ShortestPathToPoint(GridModel.HighlightedCell.model.PositionPoint),Color.green * new Color(1,1,1,0.5f));
		//}
	}

	void HandleOnUnitHPEqualsZero (HorizonUnitModel unit)
	{
		if(unit == GridModel.SelectedUnit)
			GridModel.SelectedUnit = null;
	}

	void HandleOnWin (HorizonGridModel model)
	{
		if(model == GridModel)
			Application.LoadLevel(1);
	}

	void HandleOnLoss (HorizonGridModel model)
	{
		if(model == GridModel)
			Application.LoadLevel(2);
	}

	bool HandleOnUnitSelected (HorizonUnitModel Unit)
	{
		if(CombatUI.Instance.attackButton.Selecting)
		{
			if(GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(Unit.PositionPoint))
			{
				if(GridModel.HighlightedCell != null && GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(GridModel.HighlightedCell.model.PositionPoint))
					GridView.popHighlightSet();

				CombatUI.Instance.attackButton.TargetUnit(Unit);
			}

			return false;
		}

		if(GridModel.SelectedUnit != null && GridModel.HighlightedCell != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			GridView.popHighlightSet();

		GridView.popHighlightSet();



		if(Unit != null)
		{
			if(Unit == GridModel.ActiveUnit && Unit.unitType == UnitType.Enemy)
			{
				GridModel.SelectedUnit = null;
				StartCoroutine(enemyAiRoutine());
				return false;
			}

			GridView.pushHighlightSet(Unit.PointsInMovmentRange,Color.red * new Color(1,1,1,0.5f));

			if(Unit != null && GridModel.HighlightedCell != null && Unit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			{
				GridView.pushHighlightSet(
					Unit.ShortestPathToPoint(GridModel.HighlightedCell.model.PositionPoint), 
					Color.green * new Color(1,1,1,0.5f)
				);
			}
		}

		return true;
	}

	IEnumerator enemyAiRoutine()
	{
		yield return 0;

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

	void HandleOnActiveUnitChanged (HorizonUnitModel NewUnit)
	{
		GridModel.HighlightedCell = null;
		GridModel.HighlightedUnit = null;
		GridModel.SelectedCell = null;

		activeUnitType = NewUnit.unitType;
		GridView.handleInput = activeUnitType == UnitType.Character;

		//GridModel.SelectedUnit = NewUnit;
	}

	void HandleOnHighlightedUnitChanged (HorizonUnitModel NewUnit)
	{
		if(GridModel.HighlightedUnit != null)
		{
			GridModel.HighlightedUnit.gameObject.GetComponent<HorizonUnitGameView>().OutlinePercentage = 0;
		}

		if(NewUnit != null && NewUnit != GridModel.SelectedUnit)
		{
			NewUnit.gameObject.GetComponent<HorizonUnitGameView>().OutlinePercentage = 80;
		}
	}

	void MoveSelectedUnitTo(RectPoint point)
	{
		HorizonUnitModel unit = GridModel.SelectedUnit;

		//unselect the unit
		GridModel.SelectedUnit = null;
		//disable user input
		GridView.handleInput = false;

		Action OnMoveEnd;
		OnMoveEnd = () => 
		{
			unit.OnTraversePathEnd -= OnMoveEnd;
			//enable user input
			GridView.handleInput = true;
			GridModel.SelectedUnit = GridModel.ActiveUnit;
		};

		unit.OnTraversePathEnd += OnMoveEnd;

		StartCoroutine(unit.TraverseShortestPathToPoint(point));
	}

	void HandleOnCellSelected (HorizonCellGameView Cell)
	{
		if(CombatUI.Instance.attackButton.Selecting)
		{
			if(GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint).Contains(Cell.model.PositionPoint) 
			   && Cell.model.OccupyingUnit != null && Cell.model.OccupyingUnit.unitType != GridModel.SelectedUnit.unitType)
			{
				GridModel.SelectedUnit = Cell.model.OccupyingUnit;
			}
		}
		else if(Cell != null && GridModel.SelectedUnit != null && GridModel.SelectedUnit == GridModel.ActiveUnit && GridModel.SelectedUnit.PointsInMovmentRange.Contains(Cell.model.PositionPoint))
		{
			//move unit
			MoveSelectedUnitTo(Cell.model.PositionPoint);
		}
	}

	
	void HandleOnHighlightedCellChanged (HorizonCellGameView NewCell)
	{
		if(CombatUI.Instance.attackButton.Selecting)
		{
			IEnumerable<RectPoint> pointsInAttackRange = GridModel.CellViewGrid.GetNeighbors(GridModel.SelectedUnit.PositionPoint);

			if(GridModel.HighlightedCell != null && pointsInAttackRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
				GridView.popHighlightSet();

			if(NewCell != null && pointsInAttackRange.Contains(NewCell.model.PositionPoint))
				GridView.pushHighlightSet(new List<RectPoint>{NewCell.model.PositionPoint},Color.cyan * new Color(1,1,1,0.8f));

			return;
		}

		if(GridModel.SelectedUnit != null && GridModel.SelectedUnit == GridModel.ActiveUnit && GridModel.HighlightedCell != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			GridView.popHighlightSet();

		if(NewCell != null)
		{
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
