using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public class ModelBase : MonoBehaviour, INotifyPropertyChanged, ISerializationCallbackReceiver, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<EventArgs> StartEvent;
		public readonly EventName StartEventName;

		public event EventHandler<EventArgs> UpdateEvent;
		public readonly EventName UpdateEventName;

		public event EventHandler<EventArgs> LateUpdateEvent;
		public readonly EventName LateUpdateEventName;

		public event EventHandler<EventArgs> OnDrawGizmosEvent;
		public readonly EventName OnDrawGizmosEventName;

		public event EventHandler<EventArgs> OnDrawGizmosSelectedEvent;
		public readonly EventName OnDrawGizmosSelectedEventName;

		public ModelBase()
		{
			StartEventName = this.GetEventNameFromExpresion(() => StartEvent);
			UpdateEventName = this.GetEventNameFromExpresion(() => UpdateEvent);
			LateUpdateEventName = this.GetEventNameFromExpresion(() => LateUpdateEvent);
			OnDrawGizmosEventName = this.GetEventNameFromExpresion(() => OnDrawGizmosEvent);
			OnDrawGizmosSelectedEventName = this.GetEventNameFromExpresion(() => OnDrawGizmosSelectedEvent);
			CallInitSafe();
		}

		protected virtual void Init()
		{
#if UNITY_EDITOR
			// get rid of null subscribers ... this is probably just a patch for a bug
			m_views = m_views.Where(x => x != null).ToList();

			//using reflection, find all automatic subscribers and set them up. hmm ... how does this intereact with serilization
			Type viewType = typeof(ViewBase<ModelBase>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes())
				{
					if (type.IsSubclassOf(viewType)) 
					{
						if(m_views.Any(x => x.GetType() == type) == false)
						{
							ViewBaseNonGeneric view = (ViewBaseNonGeneric)ScriptableObject.CreateInstance(type);
							view.SetModel(this);
							m_views.Add(view);
						}
						else
						{
							for(int i = 0; i < m_views.Count; i++)
							{
								if(m_views[i].GetType() == type)
								{
									if(object.ReferenceEquals(m_views[i].GetModel(), this) == false)
										m_views[i] = Instantiate(m_views[i]);

									m_views[i].SetModel(this);
								}
							}
						}
					}
				}
			}
#endif
		}

		#region ISerializationCallbackReceiver implementation
		public void OnBeforeSerialize ()
		{
			CallInitSafe();
		}
		public void OnAfterDeserialize ()
		{
			m_initilized = false;
			CallInitSafe();
		}
		#endregion
		
		private void OnDestroy()
		{
			Dispose();
		}
		
		private void CallInitSafe()
		{
			Dispatcher.CallOnMainThread (() =>  
			{
				if (!m_initilized)
				{
					Init ();
					m_initilized = true;
				}
			});
		}
		
		private bool m_initilized = false;

		protected virtual void Start()
		{
			StartEvent.FireIfNotNull(this, EventArgs.Empty);
		}

		protected virtual void Update()
		{
			UpdateEvent.FireIfNotNull(this, EventArgs.Empty);
		}

		protected virtual void LateUpdate()
		{
			LateUpdateEvent.FireIfNotNull(this, EventArgs.Empty);
		}

		private void OnDrawGizmos()
		{
			OnDrawGizmosEvent.FireIfNotNull(this, EventArgs.Empty);
		}
		
		private void OnDrawGizmosSelected()
		{
			OnDrawGizmosSelectedEvent.FireIfNotNull(this, EventArgs.Empty);
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

		public virtual void Dispose ()
		{
			foreach(ViewBaseNonGeneric subscriber in m_views)
			{
				this.DisposeAndDestroy(subscriber);
			}
		}

		// just to maintain a referance to the views
		[SerializeField]
		private List<ViewBaseNonGeneric> m_views = new List<ViewBaseNonGeneric>();
	}
}

