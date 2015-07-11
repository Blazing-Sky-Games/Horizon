using Horizon.Core;

namespace Horizon.Core.Editor
{
	public class SceneViewBase<ModelType> :  AutomaticallySubscribeTo<ModelType> 
		where ModelType:HorizonGameObjectBase
	{
		public SceneViewBase(ModelType model):base(model)
		{
			HorizonObject.DrawGizmosEvent += OnDrawGizmos;
			HorizonObject.DrawGizmosSelectedEvent += OnDrawGizmosSelected;
		}

		// called every frame in the scene view
		protected virtual void OnDrawGizmos(){}

		// called every frame in the scene view while the object is selected
		protected virtual void OnDrawGizmosSelected(){}
	}
}