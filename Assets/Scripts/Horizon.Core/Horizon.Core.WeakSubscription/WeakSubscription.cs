using System;
using System.ComponentModel;
using System.Reflection;
using Horizon.Core.ExtensionMethods;
using System.Linq.Expressions;

namespace Horizon.Core.WeakSubscription
{
	public class WeakEventSubscription<TSource, TEventArgs> : IDisposable
			where TSource : class
			where TEventArgs : EventArgs
	{
		private readonly WeakReference _targetReference;
		private readonly Object _mockTarget;
		private readonly WeakReference _sourceReference;
		private readonly MethodInfo _eventHandlerMethodInfo;
		private readonly EventInfo _sourceEventInfo;
		private readonly Delegate _ourEventHandler;
		private bool _subscribed;
				
		public WeakEventSubscription( TSource source, string sourceEventName, EventHandler<TEventArgs> targetEventHandler)
			: this(source, typeof (TSource).GetEvent(sourceEventName), targetEventHandler)
		{
		}
				
		public WeakEventSubscription(
			TSource source,
			EventInfo sourceEventInfo,
			EventHandler<TEventArgs> targetEventHandler)
		{
			if (source == null)
				throw new ArgumentNullException();
			
			if (sourceEventInfo == null)
				throw new ArgumentNullException("sourceEventInfo","missing source event info in WeakEventSubscription");
			
			_eventHandlerMethodInfo = targetEventHandler.Method;

			//hmmm this whole mock target thing might just be a patch for a bug ...
			if(targetEventHandler.Target != null)
			{
				_targetReference = new WeakReference(targetEventHandler.Target);
			}
			else
			{
				_mockTarget = new object();
				_targetReference = new WeakReference(_mockTarget);
			}
			
			_sourceReference = new WeakReference(source);
			_sourceEventInfo = sourceEventInfo;
			
			_ourEventHandler = CreateEventHandler();
			
			AddEventHandler();
		}
		
		protected virtual Delegate CreateEventHandler()
		{
			return new EventHandler<TEventArgs>(OnSourceEvent);
		}
		
		//This is the method that will handle the event of source.
		protected void OnSourceEvent(object sender, TEventArgs e)
		{
			var target = _targetReference.Target;
			if (target != null)
			{
				_eventHandlerMethodInfo.Invoke(target, new[] {sender, e});
			}
			else
			{
				RemoveEventHandler();
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				RemoveEventHandler();
			}
		}
		
		private void RemoveEventHandler()
		{
			if (!_subscribed)
				return;
			
			var source = (TSource) _sourceReference.Target;
			if (source != null)
			{
				_sourceEventInfo.GetRemoveMethod().Invoke(source, new object[] { _ourEventHandler });
				_subscribed = false;
			}
		}
		
		private void AddEventHandler()
		{
			if (_subscribed)
				throw new Exception("Should not call _subscribed twice");
			
			var source = (TSource) _sourceReference.Target;
			if (source != null)
			{
				_sourceEventInfo.GetAddMethod().Invoke(source, new object[] { _ourEventHandler });
				_subscribed = true;
			}
		}
	}
	
	public class NotifyPropertyChangedEventSubscription : WeakEventSubscription<INotifyPropertyChanged, PropertyChangedEventArgs>
	{
		private static readonly EventInfo PropertyChangedEventInfo = typeof(INotifyPropertyChanged).GetEvent("PropertyChanged");
		
		// This code ensures the PropertyChanged event is not stripped by linker. wait, what?
		public static void LinkerPleaseInclude(INotifyPropertyChanged iNotifyPropertyChanged)
		{
			iNotifyPropertyChanged.PropertyChanged += (sender, e) => { };
		}
		
		public NotifyPropertyChangedEventSubscription(INotifyPropertyChanged source,
		                                              EventHandler<PropertyChangedEventArgs> targetEventHandler)
			: base(source, PropertyChangedEventInfo, targetEventHandler)
		{
		}
		
		protected override Delegate CreateEventHandler()
		{
			return new EventHandler<PropertyChangedEventArgs>(OnSourceEvent);
		}
	}
	
	public class NamedNotifyPropertyChangedEventSubscription<T> : NotifyPropertyChangedEventSubscription
	{
		private readonly string _propertyName;
		
		public NamedNotifyPropertyChangedEventSubscription(INotifyPropertyChanged source,
		                                                   Expression<Func<T>> property,
		                                                   EventHandler<PropertyChangedEventArgs> targetEventHandler)
			: this(source, source.GetPropertyNameFromExpression(property), targetEventHandler)
		{
		}
		
		public NamedNotifyPropertyChangedEventSubscription(INotifyPropertyChanged source,
		                                                   string propertyName,
		                                                   EventHandler<PropertyChangedEventArgs> targetEventHandler)
			: base(source, targetEventHandler)
		{
			_propertyName = propertyName;
		}
		
		protected override Delegate CreateEventHandler()
		{
			return new PropertyChangedEventHandler(
				(sender, e) =>
				{
					if (string.IsNullOrEmpty(e.PropertyName)
					    || e.PropertyName == _propertyName)
					{
						OnSourceEvent(sender, e);
					}
				}
			);
		}
	}

	public class GeneralEventSubscription
		: WeakEventSubscription<object, EventArgs>
	{
		public GeneralEventSubscription(object source,
		                                   EventInfo eventInfo,
		                                   EventHandler<EventArgs> eventHandler)
			: base(source, eventInfo, eventHandler)
		{
		}
		
		protected override Delegate CreateEventHandler()
		{
			return new EventHandler<EventArgs>(OnSourceEvent);
		}
	}

	public class EventName
	{
		public readonly string Name;
		public EventName(object source, Expression<Func<EventHandler<EventArgs>>> eventExpression)
		{
			Name = source.GetEventNameStringFromExpresion(eventExpression);
		}
	}
	
	public static class WeakSubscriptionExtensionMethods
	{
		public static NamedNotifyPropertyChangedEventSubscription<T> WeakSubscribeToProperty<T>(this INotifyPropertyChanged source,
		                                                                              Expression<Func<T>> property,
		                                                                              EventHandler<PropertyChangedEventArgs>
		                                                                              eventHandler)
		{
			return new NamedNotifyPropertyChangedEventSubscription<T>(source, property, eventHandler);
		}

		public static GeneralEventSubscription WeakSubscribeToEvent(this object source, EventName eventName, EventHandler<EventArgs> eventHandler)
		{
			if(eventName == null) throw new ArgumentNullException("eventName","you are trying to subscribe to an event that has not been initilized yet");

			return new GeneralEventSubscription(source, source.GetType().GetEvent(eventName.Name), eventHandler);
		}
	}
}
