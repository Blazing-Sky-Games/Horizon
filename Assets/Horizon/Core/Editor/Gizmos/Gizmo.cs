using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor.Gizmos
{
	//inherit from this if you want to bundle up the code for a type of gizmo into a class
	public abstract class Gizmo : IDisposable
	{
		public bool Enabled = true;
		public Color color;

		public Gizmo(ModelBase gameObject, bool onlyOnSelected = false)
		{
			this.gameObject = gameObject;

			if(onlyOnSelected)
			{
				gameObject.WeakSubscribeToEvent(gameObject.OnDrawGizmosSelectedEventName, (sender,args) => Draw());
			}
			else
			{
				gameObject.WeakSubscribeToEvent(gameObject.OnDrawGizmosEventName, (sender,args) => Draw());
			}
		}

		protected virtual void Draw()
		{
			UnityEngine.Gizmos.color = color;
		}

		public virtual void Dispose (){}

		protected ModelBase gameObject;
	}
}

