using Horizon.Combat.Models; 
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using UnityEngine;
using System.Collections;
using System;
using Horizon.Core.WeakSubscription;

namespace Horizon.Combat.Models
{
	public class CombatModel : ModelBase
	{
		//set this manually in the inspector
		public Grid grid;

		protected override void Start ()
		{
			base.Start ();
			StartCoroutine (CombatRoutine ());
		}

		public IEnumerator CombatRoutine()
		{
			//select unit then select cell to move unit
			bool? PauseUnitSelection = false;
			StartCoroutinePausable (SelectUnit(), out PauseUnitSelection);
			bool? PauseCellSelection = false;
			StartCoroutinePausable (MoveSelectedUnitToCellOnClick (), out PauseCellSelection);

			yield break;
		}

		//makes unit clicked on the selected unit
		//highlights the cell underneeth the selected unit
		public IEnumerator SelectUnit()
		{
			//highlight whatever unit the mouse is over
			Unit HighlightedUnit = MouseOverUnit;
			if(HighlightedUnit != null) HighlightedUnit.Highlighted = true;

			//run until canceled
			while (true)
			{
				//the mouse has moved over a new unit, so update highlighting
				if(PropertyChangedLastFrame(()=>MouseOverUnit))
				{
					//unhighlight the old unit
					if(HighlightedUnit != null) HighlightedUnit.Highlighted = false;

					//only highlight unselected units
					if(m_mouseOverUnit != SelectedUnit)
					{
						// highligh the unit the mouse is over
						HighlightedUnit = m_mouseOverUnit;
						if(HighlightedUnit != null) HighlightedUnit.Highlighted = true;
					}
				}

				//change unit selection on click
				if(UnitClickedLastFrame ())
				{
					//change the selected unit, and highlight the cell it is in
					SelectedUnit = LastClickedUnit;

					//highlight the cell the unit is over
					if(SelectedUnit != null) grid [SelectedUnit.GridPosition].EnableHighlightState (LogicalHighlightState.EffectRange);

					//unhighlight the selected unit
					if(HighlightedUnit != null) HighlightedUnit.Highlighted = false;
					HighlightedUnit = null;
				}

				//has the cell the select unit is in changed?
				// if it has, we need to un highlight the old cell

				Unit selectedUnitLastTurn = null;
				GridPoint gridPosLastTurn = default(GridPoint);

				//if the selected unit has changed
				if(PropertyChangedLastFrame(()=>SelectedUnit, out selectedUnitLastTurn) && selectedUnitLastTurn != null)
				{
					//if that units cell has changed, unhilight its old cell
					if(selectedUnitLastTurn.PropertyChangedLastFrame(()=>selectedUnitLastTurn.GridPosition, out gridPosLastTurn))
					{
						grid [gridPosLastTurn].DisableHighlightState (LogicalHighlightState.EffectRange);
					}
					// un highlight the cell the old unit is currently in
					else
					{
						grid [selectedUnitLastTurn.GridPosition].DisableHighlightState (LogicalHighlightState.EffectRange);
					}
				}
				//the unit did not change, but di its cell change?
				else if(SelectedUnit != null && SelectedUnit.PropertyChangedLastFrame(()=>SelectedUnit.GridPosition, out gridPosLastTurn))
				{
					//unhighlight old cell
					grid [gridPosLastTurn].DisableHighlightState (LogicalHighlightState.EffectRange);
					//highlight current cell
					grid [SelectedUnit.GridPosition].EnableHighlightState (LogicalHighlightState.EffectRange);
				}

				// wait a frame
				yield return null;
			}
		}

		//wait until a unit is selected
		//then wait for a cell click, and move that unit to that cell
		//then unselect the unit
		public IEnumerator MoveSelectedUnitToCellOnClick()
		{
			//run untill canceled
			while (true)
			{
				// wait for a unit to be selected
				while(m_selectedUnit == null)
				{
					//wait a frame
					yield return null;
				}
				
				// highlight the cell the mouse is over
				Cell highlightedCell = MouseOverCell;
				if(highlightedCell != null) highlightedCell.EnableHighlightState(LogicalHighlightState.MovementPath);

				//wait for a cell to be clicked
				//TODO handle selected unit becoming null
				while (CellClickedLastFrame() == false)
				{
					//if we have moused over a new cell
					if(PropertyChangedLastFrame(()=>MouseOverCell))
					{
						//unhighlight the cell we were over
						if(highlightedCell != null) highlightedCell.DisableHighlightState(LogicalHighlightState.MovementPath);

						//highlight the cell we are now over
						highlightedCell = MouseOverCell;
						if(highlightedCell != null) highlightedCell.EnableHighlightState(LogicalHighlightState.MovementPath);
					}
					
					yield return null;
				}

				//cell has been clicked

				//unhighlight the cell we were over
				highlightedCell.DisableHighlightState(LogicalHighlightState.MovementPath);

				//move the selected unit
				SelectedUnit.GridPosition = MouseOverCell.GridPosition;

				//unselect unit
				SelectedUnit = null;

				//wait a frame
				yield return null;
			}
		}

		[HideInInspector]
		public Unit SelectedUnit
		{
			get
			{
				return m_selectedUnit;
			}
			set
			{
				//change the selected unit, and highlight the cell it is in
				SetPropertyFeild(ref m_selectedUnit,value,()=>SelectedUnit);
			}
		}
		private Unit m_selectedUnit;

		[HideInInspector]
		public Cell MouseOverCell
		{
			set
			{
				SetPropertyFeild(ref m_mouseOverCell,value,()=>MouseOverCell);
			}
			get
			{
				return m_mouseOverCell;
			}
		}
		private Cell m_mouseOverCell;
		
		[HideInInspector]
		public Unit MouseOverUnit
		{
			get
			{
				return m_mouseOverUnit;
			}
			set
			{
				SetPropertyFeild(ref m_mouseOverUnit, value, () => MouseOverUnit);
			}
		}
		private Unit m_mouseOverUnit;

		[HideInInspector]
		public Cell LastClickedCell
		{
			get
			{

				return m_lastclickedCell;
			}
			set
			{
				SetPropertyFeild(ref m_lastclickedCell, value,()=>LastClickedCell);
			}
		}
		private Cell m_lastclickedCell;

		[HideInInspector]
		public Unit LastClickedUnit
		{
			get
			{
				return m_lastClickedUnit;
			}
			set
			{
				SetPropertyFeild(ref m_lastClickedUnit, value,()=>LastClickedUnit);
			}
		}
		private Unit m_lastClickedUnit;

		public bool UnitClickedLastFrame ()
		{
			return PropertyChangedLastFrame (() => LastClickedUnit) == true && LastClickedUnit != null;
		}
		
		public bool CellClickedLastFrame ()
		{
			return PropertyChangedLastFrame (() => LastClickedCell) == true && LastClickedCell != null;
		}
	}
}
