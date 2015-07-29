using System;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor
{
	public class SceneView<HorizonObjectType> :  ViewBase<HorizonObjectType> 
		where HorizonObjectType:ModelBase
	{
		protected override void Init ()
		{
			base.Init ();
			model.WeakSubscribeToEvent(model.OnDrawGizmosEventName, (sender,args) => SceneViewUpdate());
			model.WeakSubscribeToEvent(model.OnDrawGizmosSelectedEventName, (sender,args) => WhileSelected());
		}

		protected virtual void SceneViewUpdate(){}
		protected virtual void WhileSelected(){}
	}
}