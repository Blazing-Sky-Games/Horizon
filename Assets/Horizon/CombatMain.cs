﻿using UnityEngine;

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
		GridModel.OnCellSelected += HandleOnCellSelected;;
		GridModel.OnHighlightedCellChanged += HandleOnHighlightedCellChanged;

		GridModel.OnUnitSelected += HandleOnUnitSelected;
		GridModel.OnHighlightedUnitChanged += HandleOnHighlightedUnitChanged;
		GridModel.OnActiveUnitChanged += HandleOnActiveUnitChanged;
	}

	void HandleOnUnitSelected (HorizonUnitModel Unit)
	{
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
			
	}

	void HandleOnActiveUnitChanged (HorizonUnitModel NewUnit)
	{
		// nothing yet
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
		};

		unit.OnTraversePathEnd += OnMoveEnd;

		StartCoroutine(unit.TraverseShortestPathToPoint(point));
	}

	void HandleOnCellSelected (HorizonCellView Cell)
	{
		//move unit
		if(Cell != null && GridModel.SelectedUnit != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(Cell.model.PositionPoint))
		{
			MoveSelectedUnitTo(Cell.model.PositionPoint);
		}
	}

	
	void HandleOnHighlightedCellChanged (HorizonCellView NewCell)
	{
		if(GridModel.SelectedUnit != null && GridModel.HighlightedCell != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(GridModel.HighlightedCell.model.PositionPoint))
			GridView.popHighlightSet();

		if(NewCell != null)
		{
			if(GridModel.SelectedUnit != null && GridModel.SelectedUnit.PointsInMovmentRange.Contains(NewCell.model.PositionPoint))
			{
				GridView.pushHighlightSet(
					GridModel.SelectedUnit.ShortestPathToPoint(NewCell.model.PositionPoint), 
					Color.green * new Color(1,1,1,0.5f)
				);
			}
		}
	}
}