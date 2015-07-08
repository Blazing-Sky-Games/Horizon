using System;
using UnityEngine;
using Horizon.Models;

namespace Horizon.Views
{
	public class HorizonBaseView<ModelType> : IDisposable  
		where ModelType:HorizonBaseModel
	{
		public virtual void Dispose(){}

		public HorizonBaseView(ModelType model)
		{
			Model = model;
		}

		// accses to the model for this view
		protected readonly ModelType Model;

		protected void DisposeSubscription(ref NotifyPropertyChangedEventSubscription member)
		{
			if(member!=null) member.Dispose();
			member = null;
		}
	}
}


