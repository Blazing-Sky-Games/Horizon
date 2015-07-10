using UnityEngine;
using Horizon.Core.Objects;

namespace Horizon.Core.Views.GameViews
{
	public class HorizonBaseGameView<ModelType> :  HorizonBaseView<ModelType> 
		where ModelType:HorizonGameObjectBase 
	{
		public HorizonBaseGameView(ModelType instance) : base(instance)
		{
		}
	}
}