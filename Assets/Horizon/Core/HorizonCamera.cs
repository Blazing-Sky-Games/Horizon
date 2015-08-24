using System;
using Horizon.Core;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;
using UnityEngine;

namespace Horizon.Core
{
	//wraps all rendering call backs so that they get called in the editor
	[ExecuteInEditMode]
	public class RenderCallBacks : MonoBehaviour
	{
		//subscribe to this event to do GL drawing
		public event EventHandler<EventArgs> PostRenderEvent;

		private void OnPostRender()
		{
			if(PostRenderEvent != null)
				PostRenderEvent(this,null);
		}
	}

	//base class of all horizon specific cameras
	public class HorizonCamera : ModelBase
	{
		public RenderCallBacks renderCallbacks
		{
			get
			{
				if(gameObject.GetComponent<RenderCallBacks>() == null) gameObject.AddComponent<RenderCallBacks>();

				return gameObject.GetComponent<RenderCallBacks>();
			}
		}

		//referance the main horizon camera in the scene
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

		private static HorizonCamera m_mainCamera;
	}
}

