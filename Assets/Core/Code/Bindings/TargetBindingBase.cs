using Slash.Unity.DataBind.Core.Data;

namespace Slash.Unity.DataBind.Foundation
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Reflection;

	using Slash.Unity.DataBind.Core.Presentation;
	using Slash.Unity.DataBind.Core.Utils;

	using UnityEngine;

	public abstract class TargetBindingBase : MonoBehaviour
	{
		public UnityEngine.Object Target;

		private DataNode root;

		protected virtual void Awake()
		{
			root = new DataNode(Target);
		}

		public object RegisterListener(string path, Action<object> onValueChanged)
		{
			var node = this.root.FindDescendant(path);
			if (node == null)
			{
				throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
			}

			// Register for value change.
			node.ValueChanged += onValueChanged;

			return node.Value;
		}

		public void RemoveListener(string path, Action<object> onValueChanged)
		{
			var node = this.root.FindDescendant(path);
			if (node == null)
			{
				throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
			}

			// Remove from value change.
			node.ValueChanged -= onValueChanged;
		}

		public void SetValue(string path, object value)
		{
			var node = this.root.FindDescendant(path);
			if (node == null)
			{
				throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
			}

			node.SetValue(value);
		}
	}

	public class TargetPathAttribute : PropertyAttribute
	{
		public TargetPathAttribute()
		{
			this.Filter = ContextMemberFilter.All;
		}
			
		public ContextMemberFilter Filter { get; set; }

		public Type NodeType { get; set;}

		public string PathDisplayName { get; set; }
	}

	public abstract class TargetBinding<ValueType> : TargetBindingBase
	{
		[ContextPath(Filter = ContextMemberFilter.Methods | ContextMemberFilter.Recursive, PathDisplayName = "Handler Path")]
		public string contextPath;
		private ContextNode contextNode;
		private ValueType command;

		[TargetPathAttribute(Filter = ContextMemberFilter.Fields, NodeType = typeof(IGenericMessage), PathDisplayName = "Message Path")]
		public string targetPath;
		private TargetNode targetNode;
		private IGenericMessage msg;
		DataBinding d;
	}
}

