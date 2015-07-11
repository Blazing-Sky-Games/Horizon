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

		public static HorizonCamera Main
		{
			get
			{
				return m_mainCamera;
			}
		}

		public HorizonCamera()
		{
			m_mainCamera = this;
			PostRenderEventName = this.GetEventNameFromExpresion(() => PostRenderEvent);
		}

		private void OnPostRender()
		{
			if(PostRenderEvent != null)
				PostRenderEvent(this,null);
		}

		private static HorizonCamera m_mainCamera;
	}
}

