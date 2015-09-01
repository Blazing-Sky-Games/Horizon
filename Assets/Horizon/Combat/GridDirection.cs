using System;
using UnityEngine;
using Horizon.Core;

namespace Horizon.Combat
{
	public enum GridDirection
	{
		North,East,South,West
	}

	public static class GridDirectionExtentions
	{
		public static Quaternion Rotation(this GridDirection direction)
		{
			switch (direction)
			{
			case GridDirection.East: return Quaternion.FromToRotation(Vector3.forward,Vector3.right);
			case GridDirection.South: return Quaternion.AngleAxis(180,Vector3.up);
			case GridDirection.West: return Quaternion.FromToRotation(Vector3.forward,Vector3.left);
			case GridDirection.North: // north is default
			default: return Quaternion.identity;
			}
		}
	}
}
