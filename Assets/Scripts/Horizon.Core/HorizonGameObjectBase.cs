using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Core
{
	public class HorizonGameObjectBase : MonoBehaviour, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		// make these events and create a way to week subscribe to events
		public Action DrawGizmosEvent;
		public Action DrawGizmosSelectedEvent;
		//public Action PostRenderEvent;

		public HorizonGameObjectBase()
		{
			Type viewType = typeof(AutomaticallySubscribeTo<HorizonGameObjectBase>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});
			//only get scene views in editor
			//Type sceneViewType = typeof(Horiz AutomaticallySubscribeTo<HorizonGameObjectBase>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});
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

		protected virtual void Start()
		{
			
		}

		// property stuff
		protected bool SetPropertyFeild<T>(ref T storage, T value, Expression<Func<T>> property)
		{
			return SetProperty(ref storage,value,this.GetPropertyNameFromExpression(property));
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

