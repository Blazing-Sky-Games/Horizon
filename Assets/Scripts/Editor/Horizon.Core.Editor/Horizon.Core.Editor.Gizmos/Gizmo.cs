using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

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
				gameObject.WeakSubscribeToEvent(gameObject.DrawGizmosSelectedEventName, (sender,args) => Draw());
			}
			else
			{
				gameObject.WeakSubscribeToEvent(gameObject.DrawGizmosEventName, (sender,args) => Draw());
			}
		}

		protected virtual void Draw()
		{
			UnityEngine.Gizmos.color = color;
		}

		protected HorizonGameObjectBase gameObject;
	}
}

