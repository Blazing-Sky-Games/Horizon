using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public class HorizonGameObjectBase : MonoBehaviour, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<EventArgs> DrawGizmosEvent;
		public readonly EventName DrawGizmosEventName;

		public event EventHandler<EventArgs> DrawGizmosSelectedEvent;
		public readonly EventName DrawGizmosSelectedEventName;

		public HorizonGameObjectBase()
		{
			DrawGizmosEventName = this.GetEventNameFromExpresion(() => DrawGizmosEvent);
			DrawGizmosSelectedEventName = this.GetEventNameFromExpresion(() => DrawGizmosSelectedEvent);

			//using reflection, find all automatic subscribers and set them up
			Type subscriberType = typeof(AutomaticallySubscribeTo<HorizonGameObjectBase>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});
			//TODO dont hook up scene views if we are not in the editor
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes())
				{
					if (type.IsSubclassOf(subscriberType)) 
					{
						m_subscribers.Add(Activator.CreateInstance(type,new System.Object[]{this}));
					}
				}
			}
		}

		protected virtual void Awake()
		{
			//fire awake event
		}

		protected virtual void Start()
		{
			//fire start event
		}

		protected virtual void Update()
		{
			//fire update event
		}

		protected virtual void LateUpdate()
		{
			//fire Lateupdate event
		}

		private void OnDrawGizmos()
		{
			if (DrawGizmosEvent != null) DrawGizmosEvent(this, EventArgs.Empty);
		}
		
		private void OnDrawGizmosSelected()
		{
			if (DrawGizmosSelectedEvent != null) DrawGizmosSelectedEvent(this, EventArgs.Empty);
		}

		protected bool SetPropertyFeild<T>(ref T storage, T value, Expression<Func<T>> property)
		{
			return SetProperty(ref storage,value,this.GetPropertyNameFromExpression(property));
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
		
		private void RaisePropertyChanged<T>(Expression<Func<T>> property)
		{
			var name = this.GetPropertyNameFromExpression(property);
			RaisePropertyChanged(name);
		}

		private void RaiseAllPropertiesChanged()
		{
			var changedArgs = new PropertyChangedEventArgs(string.Empty);
			RaisePropertyChanged(changedArgs);
		}

		// just to maintain a referance to the views
		private List<object> m_subscribers = new List<object>();
	}
}

