//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using Horizon.Core;
using UnityEngine;
using Horizon.Core.WeakSubscription;


namespace Horizon.Combat.Models
{
	public class Cell : ModelBase
	{
		//can the cell be walked over
		public bool Passable
		{
			get
			{
				return m_passableSerilized;
			}
			set
			{
				SetPropertyFeild(ref m_passableSerilized,value,() => Passable);
			}
		}

		[HideInInspector]
		public GridPoint GridPosition
		{
			get
			{
				return m_gridPositionSerilized;
			}
			set
			{
				if(SetPropertyFeild(ref m_gridPositionSerilized, value,() => GridPosition))
				{
					gameObject.name = "(" + GridPosition.x + "," + GridPosition.y + ")";
					transform.localPosition = new Vector3(GridPosition.x + 0.5f,0,GridPosition.y + 0.5f);
				}
			}
		}

		[HideInInspector]
		public Grid grid
		{
			get
			{
				return m_gridSerilized;
			}
			set
			{
				if(SetPropertyFeild(ref m_gridSerilized, value,() => grid))
				{
					transform.localPosition = new Vector3(GridPosition.x + 0.5f,0,GridPosition.y + 0.5f);
				}
			}
		}

		[SerializeField]
		private bool m_passableSerilized;

		[SerializeField]
		private GridPoint m_gridPositionSerilized;

		[SerializeField]
		private Grid m_gridSerilized;
	}
}

