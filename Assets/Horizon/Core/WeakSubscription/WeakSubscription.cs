using System;
using System.ComponentModel;
using System.Reflection;
using Horizon.Core.ExtensionMethods;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Horizon.Core.WeakSubscription
{
	public class WeakEventSubscription<TSource, Tprop> : IDisposable
			where TSource : class
	{
		private readonly WeakReference _targetReference;
		private readonly Object _mockTarget;
		private readonly WeakReference _sourceReference;
		private readonly MethodInfo _eventHandlerMethodInfo;
		private readonly EventInfo _sourceEventInfo;
		private readonly Action<string,object,object> _ourEventHandler;
		private bool _subscribed;
				
		public WeakEventSubscription( TSource source, string sourceEventName, Action<Tprop,Tprop> targetEventHandler)
			: this(source, typeof (TSource).GetEvent(sourceEventName), targetEventHandler)
		{
		}
				
		public WeakEventSubscription(
			TSource source,
			EventInfo sourceEventInfo,
			Action<Tprop, Tprop> targetEventHandler)
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
		
		protected virtual Action<string,object,object> CreateEventHandler()
		{
			return (name,oldval,newval) => OnSourceEvent ((Tprop)oldval, (Tprop)newval);
		}
		
		//This is the method that will handle the event of source.
		protected void OnSourceEvent(Tprop oldval, Tprop newval)
		{
			var target = _targetReference.Target;
			if (target != null)
			{
				_eventHandlerMethodInfo.Invoke(target, new object[] {oldval, newval});
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
	
	public class NotifyPropertyChangedEventSubscription<T> : WeakEventSubscription<ModelBase, T>
	{
		private static readonly EventInfo PropertyChangedEventInfo = typeof(ModelBase).GetEvent("PropertyChanged");
		
		// This code ensures the PropertyChanged event is not stripped by linker. wait, what?
		public static void LinkerPleaseInclude(ModelBase iNotifyPropertyChanged)
		{
			iNotifyPropertyChanged.PropertyChanged += (name, oldval, newval) => { };
		}
		
		public NotifyPropertyChangedEventSubscription(ModelBase source,
		                                              Action<T, T> targetEventHandler)
			: base(source, PropertyChangedEventInfo, targetEventHandler)
		{
		}
		
		protected override Action<string,object,object> CreateEventHandler()
		{
			return (name,oldval,newval) => OnSourceEvent ((T)oldval, (T)newval);
		}
	}
	
	public class NamedNotifyPropertyChangedEventSubscription<T> : NotifyPropertyChangedEventSubscription<T>
	{
		private readonly string _propertyName;
		
		public NamedNotifyPropertyChangedEventSubscription(ModelBase source,
		                                                   Expression<Func<T>> property,
		                                                   Action<T,T> targetEventHandler)
			: this(source, source.GetPropertyNameFromExpression(property), targetEventHandler)
		{
		}
		
		public NamedNotifyPropertyChangedEventSubscription(ModelBase source,
		                                                   string propertyName,
		                                                   Action<T,T> targetEventHandler)
			: base(source, targetEventHandler)
		{
			_propertyName = propertyName;
		}
		
		protected override Action<string,object,object> CreateEventHandler()
		{
			return 
				(name,oldval,newval) =>
				{
					if (string.IsNullOrEmpty(name) || name == _propertyName)
					{
						OnSourceEvent((T)oldval, (T)newval);
					}
				};
		}
	}

	public static class WeakSubscriptionExtensionMethods
	{
		public static NamedNotifyPropertyChangedEventSubscription<T> WeakSubscribe<T>(this ModelBase source,
		                                                                              Expression<Func<T>> property,
		                                                                              Action<T,T> eventHandler)
		{
			return new NamedNotifyPropertyChangedEventSubscription<T>(source, property, eventHandler);
		}


	}
}
