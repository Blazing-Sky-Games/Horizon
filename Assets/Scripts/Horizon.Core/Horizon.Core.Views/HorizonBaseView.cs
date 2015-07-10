using System;
using UnityEngine;
using Horizon.Core.Objects;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Views
{
	public class HorizonBaseView<ModelType> : IDisposable  
		where ModelType:HorizonGameObjectBase
	{
		//TODO add the concept of disabling a view

		public virtual void Dispose(){}

		public HorizonBaseView(ModelType model)
		{
			Model = model;
		}

		// accses to the model for this view
		protected readonly ModelType Model;
	}
}


