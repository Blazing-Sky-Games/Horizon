using UnityEngine;
using System.Collections;
using System;

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
}
