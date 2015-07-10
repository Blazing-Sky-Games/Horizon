using Horizon.Core.Objects;

namespace Horizon.Core.Views.SceneViews
{
	public class HorizonBaseSceneView<ModelType> :  HorizonBaseView<ModelType> 
		where ModelType:HorizonGameObjectBase
	{
		public HorizonBaseSceneView(ModelType model):base(model)
		{
			Model.DrawGizmosEvent += OnDrawGizmos;
			model.DrawGizmosSelectedEvent += OnDrawGizmosSelected;
		}

		// called every frame in the scene view
		protected virtual void OnDrawGizmos(){}

		// called every frame in the scene view while the object is selected
		protected virtual void OnDrawGizmosSelected(){}
	}
}