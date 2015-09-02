using System;
using System.Linq.Expressions;
using System.Reflection;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.ExtensionMethods
{
	public static class ObjectExtentionMethods
	{
		private const string WrongExpressionMessage =
			"Wrong expression\nshould be called with expression like\n() => PropertyName";
		private const string WrongUnaryExpressionMessage =
			"Wrong unary expression\nshould be called with expression like\n() => PropertyName";
		
		public static string GetPropertyNameFromExpression<T>( this object target, Expression<Func<T>> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			
			var memberExpression = expression.FindMemberExpression();
			
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

		public static string GetEventNameStringFromExpresion( this object target, Expression<Func<EventHandler<EventArgs>>> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			
			var memberExpression = expression.FindMemberExpression();
			
			if (memberExpression == null)
			{
				throw new ArgumentException(WrongExpressionMessage, "expression");
			}
			
			EventInfo ei = target.GetType().GetEvent(memberExpression.Member.Name);
			if(ei == null) throw new ArgumentException(WrongExpressionMessage, "expression");

			return memberExpression.Member.Name;
		}
		
		public static MemberExpression FindMemberExpression<T>(this Expression<Func<T>> expression)
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

		public static void DisposeAndDestroy<T>(this object self, T toDestroy) where T: UnityEngine.Object , IDisposable
		{
			if(toDestroy != null)
			{
				toDestroy.Dispose();
				UnityEngine.Object.DestroyImmediate(toDestroy);
			}
		}
	}
}

