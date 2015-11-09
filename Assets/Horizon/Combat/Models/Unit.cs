using System;
using System.Collections.Generic;
using UnityEngine;
using Horizon.Core;

namespace Horizon.Combat.Models
{
	public class Unit : ModelBase
	{
		private static int currId = 1;
		[HideInInspector]
		public static Dictionary<int, Unit> unitLookup = new Dictionary<int, Unit> ();

		public int id;

		public AnimatedMesh animatedMesh;

		protected override void Init ()
		{
			base.Init ();

			//set up the animated mesh. you have to make sure it is not null and that its parent is set
			if(animatedMesh == null)
			{
				animatedMesh = new AnimatedMesh();
			}
			
			animatedMesh.setParent(gameObject);
		}

		protected override void Start ()
		{
			base.Start ();
			id = currId;
			currId++;
			unitLookup [id] = this;
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
				SetPropertyFeild(ref m_gridSerilized, value, () => grid);
				GridPosition = GridPosition;
			}
		}

		public GridPoint GridPosition
		{
			get
			{
				return m_positionSerilized;
			}
			set
			{
				//no grid! jump ship!
				if(grid == null) return;

				//make sure value is in bounds
				if(value.x < 0) value.x = 0;
				if(value.y < 0) value.y = 0;

				if(value.x >= grid.Dimensions.x) value.x = grid.Dimensions.x - 1;
				if(value.y >= grid.Dimensions.y) value.y = grid.Dimensions.y - 1;

				// set pos to position of cell we are in
				SetPropertyFeild(ref m_positionSerilized, value, () => GridPosition);
				transform.localPosition = grid[GridPosition.x][GridPosition.y].transform.localPosition; 
			}
		}

		public GridDirection DirectionFacing
		{
			get
			{
				return m_directionFacingSerilized;
			}
			set
			{
				SetPropertyFeild(ref m_directionFacingSerilized, value, () => DirectionFacing);
				transform.localRotation = DirectionFacing.Rotation();
			}
		}

		[HideInInspector]
		public bool Highlighted
		{
			get
			{
				return highlighted;
			}
			set
			{
				SetPropertyFeild(ref highlighted, value,()=>Highlighted);
			}
		}
		private bool highlighted;

		public override void Dispose ()
		{
			base.Dispose ();
			unitLookup.Remove (id);
		}

		[SerializeField]
		private Grid m_gridSerilized;

		[SerializeField]
		private GridPoint m_positionSerilized;

		[SerializeField]
		private GridDirection m_directionFacingSerilized;
	}
}

