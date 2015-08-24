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
	//base class of all models
	// attach a model to a gameobject, and all the related views will be attached as well
	// models also provide a way to create observable properties through weak subscribtion
	public class ModelBase : MonoBehaviour, INotifyPropertyChanged, ISerializationCallbackReceiver, IDisposable
	{
		// impliment INotifyPropertyChanged. backend that makes observable propeties work
		public event PropertyChangedEventHandler PropertyChanged;

		//events
		public event EventHandler<EventArgs> StartEvent;
		public event EventHandler<EventArgs> UpdateEvent; 
		public event EventHandler<EventArgs> LateUpdateEvent; 
		public event EventHandler<EventArgs> OnDrawGizmosEvent;
		public event EventHandler<EventArgs> OnDrawGizmosSelectedEvent;

		//events in c# are weird. the cannot be referanced outside the class the are defined in
		//event names are a way to get around that
		public readonly EventName StartEventName;
		public readonly EventName UpdateEventName;
		public readonly EventName LateUpdateEventName;
		public readonly EventName OnDrawGizmosEventName;
		public readonly EventName OnDrawGizmosSelectedEventName;

		public ModelBase()
		{
			//eventnames!
			//todo: getrid of events names, they are confusing and weird
			StartEventName = this.GetEventNameFromExpresion(() => StartEvent);
			UpdateEventName = this.GetEventNameFromExpresion(() => UpdateEvent);
			LateUpdateEventName = this.GetEventNameFromExpresion(() => LateUpdateEvent);
			OnDrawGizmosEventName = this.GetEventNameFromExpresion(() => OnDrawGizmosEvent);
			OnDrawGizmosSelectedEventName = this.GetEventNameFromExpresion(() => OnDrawGizmosSelectedEvent);
		}

		//hook up all views through reflection
		protected virtual void Init()
		{
#if UNITY_EDITOR
			// get rid of null subscribers ... this is probably just a patch for a bug
			m_views = m_views.Where(x => x != null).ToList();

			//using reflection, find all views and set them up.

			//view type is the base class of all views that attach to this type of model
			Type viewType = typeof(ViewBase<ModelBase>).GetGenericTypeDefinition().MakeGenericType(new Type[]{this.GetType()});

			//find all derived classes of viewtype
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes())
				{
					if (type.IsSubclassOf(viewType)) 
					{
						//if there is not already a view of this type attached (as a result of deserilizing)
						if(m_views.Any(x => x.GetType() == type) == false)
						{
							//creat and init a new view
							ViewBaseNonGeneric view = (ViewBaseNonGeneric)ScriptableObject.CreateInstance(type);
							view.SetModel(this);
							m_views.Add(view);

							//............
							//i dont remember what these next lines are for
							//somthing about prefabs and not leaking views
							if(gameObject == null) continue;
							if(gameObject.activeInHierarchy)
							{
								view.hideFlags = HideFlags.None;
							}
							else
							{
								view.hideFlags = HideFlags.HideAndDontSave;
							}
						}
						else // there was already a view of this type
						{
							for(int i = 0; i < m_views.Count; i++)
							{
								if(m_views[i].GetType() == type)
								{
									//if the view is pointing to the wrong model, that means this model is a copy of another model
									// so we need to copy the view, and set its model to this model
									if(object.ReferenceEquals(m_views[i].GetModel(), this) == false)
										m_views[i] = Instantiate(m_views[i]);

									m_views[i].SetModel(this);

									//............
									//i dont remember what these next lines are for
									//somthing about prefabs and not leaking views
									if(gameObject == null) continue;
									if(gameObject.activeInHierarchy)
									{
										m_views[i].hideFlags = HideFlags.None;
									}
									else
									{
										m_views[i].hideFlags = HideFlags.HideAndDontSave;
									}
								}
							}
						}
					}
				}
			}
#endif
		}


		//empty method, just there to impliment the interface
		public void OnBeforeSerialize ()
		{
		}
		public void OnAfterDeserialize ()
		{
			//if we have deserialized, we need to init
			CallInitSafe();
		}
		
		private void OnDestroy()
		{
			Dispose();
		}
		
		private void CallInitSafe()
		{
			//call init on the main thread as soon as it starts
			Dispatcher.CallOnMainThread (() =>  
			{
				//only call init if we have not already been initilized
				if (!m_initilized)
				{
					Init ();
					m_initilized = true;
				}
			});
		}
		
		[NonSerialized]
		private bool m_initilized = false; // have we been initilized?

		//unity callbacks
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


		//call set property field inside the setter of a property to make that property observable
		// calling converntion is 
		// SetPropertyFeild(ref m_backingfield, value, () => PropertyName);
		// returns a bool indicating if the value changed
		protected bool SetPropertyFeild<T>(ref T storage, T value, Expression<Func<T>> property)
		{
			return SetProperty(ref storage,value,this.GetPropertyNameFromExpression(property));
		}

		//SetPropertyFeild(ref m_backingfield, value, "PropertyName");
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

		//RaisePropertyChanged("PropertyName")
		//indicates the property has changed
		private void RaisePropertyChanged(string whichProperty = "")
		{
			var changedArgs = new PropertyChangedEventArgs(whichProperty);
			RaisePropertyChanged(changedArgs);
		}

		// fires the property changed event
		private void RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, changedArgs);
		}
		
		//RaisePropertyChanged(() => PropertyName)
		//hmmm ... make this protected if we ever need it
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

		//clean up all of the views
		public virtual void Dispose ()
		{
			foreach(ViewBaseNonGeneric subscriber in m_views)
			{
				this.DisposeAndDestroy(subscriber);
			}
		}

		// maintain referances to the views
		[SerializeField]
		private List<ViewBaseNonGeneric> m_views = new List<ViewBaseNonGeneric>();
	}
}

