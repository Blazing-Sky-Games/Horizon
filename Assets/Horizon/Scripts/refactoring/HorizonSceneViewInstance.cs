using UnityEngine;
using System.Collections;
using Horizon.Models;

namespace Horizon.Views.Scene
{
	public class HorizonSceneViewInstance : HorizonBaseSceneView<HorizonModelInstance> 
	{
		public HorizonSceneViewInstance(HorizonModelInstance model) : base(model){}

		protected override void OnDrawGizmos ()
		{
			base.OnDrawGizmos ();
			HorizonGizmos.DrawSolidRectangleGizmo(Model.transform,Color.white.SetAlpha(0.5f));
		}
	}
}


