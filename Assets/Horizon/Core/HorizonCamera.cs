using System;
using Horizon.Core;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;
using UnityEngine;

namespace Horizon.Core
{
	//base class of all horizon specific cameras
	public class HorizonCamera : ModelBase
	{
		// hook gl calls into this
		public RenderCallBacks renderCallbacks
		{
			get
			{
				if(gameObject.GetComponent<RenderCallBacks>() == null) gameObject.AddComponent<RenderCallBacks>();

				return gameObject.GetComponent<RenderCallBacks>();
			}
		}

		private static HorizonCamera m_mainCamera;
	}
}

