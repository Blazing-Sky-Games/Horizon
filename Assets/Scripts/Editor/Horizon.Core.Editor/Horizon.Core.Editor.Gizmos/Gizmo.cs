using System;
using UnityEngine;
using Horizon.Core;

namespace Horizon.Core.Editor.Gizmos
{
	public abstract class Gizmo
	{
		public bool Enabled = true;
		public Color color;

		public Gizmo(HorizonGameObjectBase gameObject, bool onlyOnSelected = false)
		{
			this.gameObject = gameObject;

			if(onlyOnSelected)
			{
				gameObject.DrawGizmosSelectedEvent += Draw;
			}
			else
			{
				gameObject.DrawGizmosEvent += Draw;
			}
		}

		protected virtual void Draw()
		{
			UnityEngine.Gizmos.color = color;
		}

		protected HorizonGameObjectBase gameObject;
	}
}

