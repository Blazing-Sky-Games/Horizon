using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Models
{
	public class HorizonBaseModel : MonoBehaviour, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Action DrawGizmosEvent;
		public Action DrawGizmosSelectedEvent;
		public Action PostRenderEvent;

		public HorizonBaseModel()
		{
			// dont do it this way ... it is lazy
			// if we change the way this works we could use views as a way to turn functionality on and off
			Type viewType = typeof(Horizon.Core.Views.HorizonBaseView<HorizonBaseModel>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes())
				{
					if (type.IsSubclassOf(viewType)) 
					{
						views.Add((IDisposable)Activator.CreateInstance(type,new System.Object[]{this}));
					}
				}
			}
		}

		//TODO add editor only method to serilize to and from a custom asset ... hmm ... maybe this should be an extention class
		
		public void RaisePropertyChanged<T>(Expression<Func<T>> property)
		{
			var name = this.GetPropertyNameFromExpression(property);
			RaisePropertyChanged(name);
		}

		public void RaiseAllPropertiesChanged()
		{
			var changedArgs = new PropertyChangedEventArgs(string.Empty);
			RaisePropertyChanged(changedArgs);
		}

		protected bool SetPropertyFeild<T>(ref T storage, T value, Expression<Func<T>> property)
		{
			return SetProperty(ref storage,value,this.GetPropertyNameFromExpression(property));
		}

		protected virtual void Start()
		{
			//Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent += PostRenderEvent;
		}
		
		private void RaisePropertyChanged(string whichProperty = "")
		{
			var changedArgs = new PropertyChangedEventArgs(whichProperty);
			RaisePropertyChanged(changedArgs);
		}
		
		private void RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, changedArgs);
		}
		
		private bool SetProperty<T>(ref T storage, T value, string propertyName = null)
		{
			if (Equals(storage, value))
			{
				return false;
			}
			
			storage = value;
			RaisePropertyChanged(propertyName);
			return true;
		}

		private void OnDrawGizmos()
		{
			if (DrawGizmosEvent != null) DrawGizmosEvent();
		}

		private void OnDrawGizmosSelected()
		{
			if (DrawGizmosSelectedEvent != null) DrawGizmosSelectedEvent();
		}

		private void OnDestroy()
		{
			foreach(IDisposable disposable in views) disposable.Dispose();
		}

		//make this alist of views so we call update/start/etc
		private List<IDisposable> views = new List<IDisposable>();
	}
}

