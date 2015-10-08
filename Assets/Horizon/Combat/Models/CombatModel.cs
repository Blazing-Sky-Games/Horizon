using Horizon.Combat.Models; 
using Horizon.Core;
using UnityEngine;

namespace Horizon.Combat.Models
{
	public class CombatModel : ModelBase
	{
		public Grid grid;

		[HideInInspector]
		public Cell MouseOverCell
		{
			set
			{
				if(mouseOverCell != null)
					mouseOverCell.HighlightState = mouseOverCell.HighlightState.DisableHighlightState(LogicalHighlightState.EffectRange);

				if(value != null)
					value.HighlightState = value.HighlightState.EnableHighlightState(LogicalHighlightState.EffectRange);

				SetPropertyFeild(ref mouseOverCell,value,()=>MouseOverCell);
			}
			get
			{
				return mouseOverCell;
			}
		}

		public void ClickCell(GridPoint clicked)
		{
			ClickCell (grid [clicked]);
		}

		public void ClickCell(Cell clicked)
		{
			//Debug.Log (clicked.name);

			if(LastClicked != null)
				LastClicked.HighlightState = LastClicked.HighlightState.DisableHighlightState (LogicalHighlightState.TargetingRange);

			if(clicked != null)
				clicked.HighlightState = clicked.HighlightState.EnableHighlightState (LogicalHighlightState.TargetingRange);

			LastClicked = clicked;
		}

		private Cell mouseOverCell;
		private Cell LastClicked;
	}
}
