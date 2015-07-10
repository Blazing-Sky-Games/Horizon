using UnityEngine;
using Horizon.Combat.Models;
using Horizon.Core.Objects;
using Horizon.Core.Views.SceneViews;
using Horizon.Core.Gizmos;
using Horizon.Core.GL;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Combat.Views.SceneViews
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


