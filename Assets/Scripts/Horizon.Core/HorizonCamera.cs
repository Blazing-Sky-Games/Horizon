using System;
using Horizon.Core;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;
using UnityEngine;

namespace Horizon.Core
{
	public class HorizonCamera : HorizonGameObjectBase
	{
		public event EventHandler<EventArgs> PostRenderEvent;
		public readonly EventName PostRenderEventName;

		public HorizonCamera()
		{
			PostRenderEventName = this.GetEventNameFromExpresion(() => PostRenderEvent);
		}

		public static HorizonCamera Main
		{
			get
			{
				return m_mainCamera;
			}
		}

		protected override void Init ()
		{
			base.Init ();
			m_mainCamera = this;
		}

		private void OnPostRender()
		{
			if(PostRenderEvent != null)
				PostRenderEvent(this,null);
		}

		private static HorizonCamera m_mainCamera;
	}
}

