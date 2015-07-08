using Horizon.Models;

namespace Horizon.Views.Scene
{
	public class HorizonBaseSceneView<ModelType> :  HorizonBaseView<ModelType> 
		where ModelType:HorizonBaseModel
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