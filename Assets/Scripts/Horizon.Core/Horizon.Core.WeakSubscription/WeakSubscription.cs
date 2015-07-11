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
				throw new ArgumentNullException("sourceEventInfo","missing source event info in MvxWeakEventSubscription");
			
			_eventHandlerMethodInfo = targetEventHandler.Method;
			_targetReference = new WeakReference(targetEventHandler.Target);
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
		
		// This code ensures the PropertyChanged event is not stripped by linker
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
			return new PropertyChangedEventHandler(OnSourceEvent);
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
	
	public static class WeakSubscriptionExtensionMethods
	{
		public static NamedNotifyPropertyChangedEventSubscription<T> WeakSubscribe<T>(this INotifyPropertyChanged source,
		                                                                              Expression<Func<T>> property,
		                                                                              EventHandler<PropertyChangedEventArgs>
		                                                                              eventHandler)
		{
			return new NamedNotifyPropertyChangedEventSubscription<T>(source, property, eventHandler);
		}
	}
}
