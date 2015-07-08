using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Horizon
{
	public static class PropertyNameExtensionMethods
	{
		private const string WrongExpressionMessage =
			"Wrong expression\nshould be called with expression like\n() => PropertyName";
		private const string WrongUnaryExpressionMessage =
			"Wrong unary expression\nshould be called with expression like\n() => PropertyName";
		
		public static string GetPropertyNameFromExpression<T>(
			this object target,
			Expression<Func<T>> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			
			var memberExpression = FindMemberExpression(expression);
			
			if (memberExpression == null)
			{
				throw new ArgumentException(WrongExpressionMessage, "expression");
			}
			
			var member = memberExpression.Member as PropertyInfo;
			if (member == null)
			{
				throw new ArgumentException(WrongExpressionMessage, "expression");
			}
			
			if (member.DeclaringType == null)
			{
				throw new ArgumentException(WrongExpressionMessage, "expression");
			}
			
			if (target != null)
			{
				if (!member.DeclaringType.IsAssignableFrom(target.GetType()))
				{
					throw new ArgumentException(WrongExpressionMessage, "expression");
				}
			}
			
			if (member.GetGetMethod(true).IsStatic)
			{
				throw new ArgumentException(WrongExpressionMessage, "expression");
			}
			
			return member.Name;
		}
		
		private static MemberExpression FindMemberExpression<T>(Expression<Func<T>> expression)
		{
			if (expression.Body is UnaryExpression)
			{
				var unary = (UnaryExpression)expression.Body;
				var member = unary.Operand as MemberExpression;
				if (member == null)
					throw new ArgumentException(WrongUnaryExpressionMessage, "expression");
				return member;
			}
			
			return expression.Body as MemberExpression;
		}
	}
	
	public class WeakEventSubscription<TSource, TEventArgs>
		: IDisposable
			where TSource : class
			where TEventArgs : EventArgs
			{
				private readonly WeakReference _targetReference;
				private readonly WeakReference _sourceReference;
				
				private readonly MethodInfo _eventHandlerMethodInfo;
				
				private readonly EventInfo _sourceEventInfo;
				
				// we store a copy of our Delegate/EventHandler in order to prevent it being
				// garbage collected while the `client` still has ownership of this subscription
				private readonly Delegate _ourEventHandler;
				
				private bool _subscribed;
				
				public WeakEventSubscription(
					TSource source,
					string sourceEventName,
					EventHandler<TEventArgs> targetEventHandler)
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
						throw new ArgumentNullException("sourceEventInfo",
						                                "missing source event info in MvxWeakEventSubscription");
					
					_eventHandlerMethodInfo = targetEventHandler.Method;
					_targetReference = new WeakReference(targetEventHandler.Target);
					_sourceReference = new WeakReference(source);
					_sourceEventInfo = sourceEventInfo;
					
					// TODO: need to move this virtual call out of the constructor - need to implement a separate Init() method
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
	
	public class NotifyPropertyChangedEventSubscription
		: WeakEventSubscription<INotifyPropertyChanged, PropertyChangedEventArgs>
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
	
	public class NamedNotifyPropertyChangedEventSubscription<T>
		: NotifyPropertyChangedEventSubscription
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
			return new PropertyChangedEventHandler((sender, e) =>
			                                       {
				if (string.IsNullOrEmpty(e.PropertyName)
				    || e.PropertyName == _propertyName)
				{
					OnSourceEvent(sender, e);
				}
			});
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
