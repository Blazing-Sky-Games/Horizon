using UnityEngine;
using Horizon.Core.Models;

namespace Horizon.Core.Views.GameViews
{
	public class HorizonBaseGameView<ModelType> :  HorizonBaseView<ModelType> 
		where ModelType:HorizonBaseModel 
	{
		public HorizonBaseGameView(ModelType model) : base(model)
		{
			Model.PostRenderEvent += OnPostRender;
		}
		
		// do all gl drawing here
		protected virtual void OnPostRender(){}
	}
}