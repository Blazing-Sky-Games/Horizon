using System;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor
{
	public class SceneView<HorizonObjectType> :  AutomaticallySubscribeTo<HorizonObjectType> 
		where HorizonObjectType:HorizonGameObjectBase
	{
		protected override void Init ()
		{
			base.Init ();
			HorizonObject.WeakSubscribeToEvent(HorizonObject.OnDrawGizmosEventName, (sender,args) => SceneViewUpdate());
			HorizonObject.WeakSubscribeToEvent(HorizonObject.OnDrawGizmosSelectedEventName, (sender,args) => WhileSelected());
		}

		protected virtual void SceneViewUpdate(){}
		protected virtual void WhileSelected(){}
	}
}