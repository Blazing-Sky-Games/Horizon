using System;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor
{
	//inherit from this class to do scene view specific stuff
	public class SceneView<HorizonObjectType> :  ViewBase<HorizonObjectType> 
		where HorizonObjectType:ModelBase
	{
		protected override void Init ()
		{
			base.Init ();
			model.WeakSubscribeToEvent(model.OnDrawGizmosEventName, (sender,args) => SceneViewUpdate());
			model.WeakSubscribeToEvent(model.OnDrawGizmosSelectedEventName, (sender,args) => WhileSelected());
		}

		protected virtual void SceneViewUpdate(){} // called everyframe in the scene view
		protected virtual void WhileSelected(){} // called while the model is selected
	}
}