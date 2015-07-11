using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor
{
	public class SceneView<HorizonObjectType> :  AutomaticallySubscribeTo<HorizonObjectType> 
		where HorizonObjectType:HorizonGameObjectBase
	{
		public SceneView(HorizonObjectType HorizonObject):base(HorizonObject)
		{
			HorizonObject.WeakSubscribeToEvent(HorizonObject.DrawGizmosEventName, (sender,args) => SceneViewUpdate());
			HorizonObject.WeakSubscribeToEvent(HorizonObject.DrawGizmosSelectedEventName, (sender,args) => WhileSelected());
		}

		protected virtual void SceneViewUpdate(){}
		protected virtual void WhileSelected(){}
	}
}