using System;
using UnityEngine;
using Horizon.Core;

namespace Horizon.Combat.Models
{
	public enum GridDirection
	{
		North,East,South,West
	}

	public class Unit : ModelBase
	{
		public AnimatedMesh animatedMesh;

		protected override void Init ()
		{
			base.Init ();

			if(animatedMesh == null)
			{
				animatedMesh = new AnimatedMesh();
			}
			
			animatedMesh.setParent(gameObject);
		}

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
				if(grid == null) return;

				if(value.x < 0) value.x = 0;
				if(value.y < 0) value.y = 0;

				if(value.x >= grid.Dimensions.x) value.x = grid.Dimensions.x - 1;
				if(value.y >= grid.Dimensions.y) value.y = grid.Dimensions.y - 1;

				SetPropertyFeild(ref m_positionSerilized, value, () => GridPosition);
				updateModelPosition();

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
				updateModelRotation();
			}
		}

		private void updateModelRotation ()
		{
			switch (DirectionFacing)
			{
			case GridDirection.North:
				transform.localRotation = Quaternion.identity;
				break;
			case GridDirection.East:
				transform.localRotation = Quaternion.FromToRotation(Vector3.forward,Vector3.right);
				break;
			case GridDirection.South:
				transform.localRotation = Quaternion.AngleAxis(180,Vector3.up);
				break;
			case GridDirection.West:
				transform.localRotation = Quaternion.FromToRotation(Vector3.forward,Vector3.left);
				break;
			}
		}

		private void updateModelPosition()
		{
			transform.localPosition = new Vector3(grid.CellSize * GridPosition.x + grid.CellSize / 2.0f, 0, grid.CellSize * GridPosition.y + grid.CellSize / 2.0f);
		}

		[SerializeField]
		private Grid m_gridSerilized;
		[SerializeField]
		private GridPoint m_positionSerilized;
		[SerializeField]
		private GridDirection m_directionFacingSerilized;
	}
}

