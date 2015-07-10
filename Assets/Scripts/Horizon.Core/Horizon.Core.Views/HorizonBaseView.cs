using System;
using UnityEngine;
using Horizon.Core.Models;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Views
{
	public class HorizonBaseView<ModelType> : IDisposable  
		where ModelType:HorizonBaseModel
	{
		//TODO add the concept of disabling a view

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


