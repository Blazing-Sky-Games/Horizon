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
				if(SetPropertyFeild(ref mouseOverCell,value,()=>MouseOverCell))
				{
					if(mouseOverCell != null)
						Debug.Log (mouseOverCell.name);
					else
						Debug.Log("null");
				}
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
			Debug.Log (clicked.name);
		}

		private Cell mouseOverCell;
	}
}
