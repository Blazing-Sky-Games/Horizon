using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Gamelogic.Grids;

public class CombatMain : MonoBehaviour 
{
	public HorizonGridView GridView;
	public HorizonGridModel GridModel;

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

	void HandleOnActiveUnitChanged (HorizonUnitModel NewUnit)
	{
		GridModel.HighlightedCell = null;
		GridModel.HighlightedUnit = null;
		GridModel.SelectedCell = null;
		GridModel.SelectedUnit = NewUnit;
	}

	void HandleOnHighlightedUnitChanged (HorizonUnitModel NewUnit)
	{
		if(GridModel.HighlightedUnit != null)
		{
			GridModel.HighlightedUnit.gameObject.GetComponent<HorizonUnitView>().OutlinePercentage = 0;
		}

		if(NewUnit != null && NewUnit != GridModel.SelectedUnit)
		{
			NewUnit.gameObject.GetComponent<HorizonUnitView>().OutlinePercentage = 80;
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

	void HandleOnCellSelected (HorizonCellView Cell)
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

	
	void HandleOnHighlightedCellChanged (HorizonCellView NewCell)
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
