using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

public sealed class MessageDataNode
{
	#region Fields

	public MessageProperty ParentObject;

	public MessageProperty Value;

	//TODO bind to message property so we can know when the value changes
	MessageProperty<MessageProperty> Property;

	#endregion

	#region Constructors and Destructors

	private MessageDataNode(NodeTypeInfo typeInfo)
	{
		this.Children = new List<MessageDataNode>();
		this.TypeInfo = typeInfo;
	}

	public MessageDataNode(MessageContext context)
	{
		this.Children = new List<MessageDataNode>();
		this.TypeInfo = new NodeTypeInfo { Type = context.GetType() };
		this.Value = new MessageProperty(context);
		Value.Changed.AddHandler(WaitUpdateChildren);
	}

	public MessageDataNode(UnityEngine.Object obj)
	{
		this.Children = new List<MessageDataNode>();
		this.TypeInfo = new NodeTypeInfo { Type = obj.GetType() };
		this.Value = new MessageProperty(obj);
		Value.Changed.AddHandler(WaitUpdateChildren);
	}

	public IEnumerator WaitUpdateChildren(object value)
	{
		// Update children parent object.
		foreach (var childNode in this.Children)
		{
			yield return new Routine(childNode.ParentObject.WaitSet(this.Value.ObjectValue));
		}
	}

	#endregion

	#region Properties

	private List<MessageDataNode> Children { get; set; }

	private string Name { get; set; }

	/// <summary>
	///   Cached type information of the data value this node capsules.
	/// </summary>
	private NodeTypeInfo TypeInfo { get; set; }

	#endregion

	#region Public Methods and Operators

	public MessageDataNode FindDescendant(string path)
	{
		var pointPos = path.IndexOf(MessageContext.PathSeparator);
		var nodeName = path;
		string pathRest = null;
		if (pointPos >= 0)
		{
			nodeName = path.Substring(0, pointPos);
			pathRest = path.Substring(pointPos + 1);
		}

		// Get children with name.
		var childNode = this.GetChild(nodeName);
		if (childNode == null)
		{
			return null;
		}

		return String.IsNullOrEmpty(pathRest) ? childNode : childNode.FindDescendant(pathRest);
	}

	public IEnumerator WaitSetValue(object newValue)
	{
		// Update data value.
		this.TypeInfo.SetValue(this.ParentObject.ObjectValue, newValue);

		// Update cached value.
		yield return new Routine(this.Value.WaitSet(newValue));
	}

	#endregion

	#region Methods

	private MessageDataNode CreateChild(string name)
	{
		// Get type of child.
		var typeInfo = this.GetChildTypeInfo(name);
		if (typeInfo == null)
		{
			// No child with this name.
			return null;
		}

		var childNode = new MessageDataNode(typeInfo) { Name = name, ParentObject = this.Value };
		childNode.Value = new MessageProperty(childNode.TypeInfo.GetValue(childNode.ParentObject));
		this.Children.Add(childNode);
		return childNode;
	}

	private MessageDataNode GetChild(string name)
	{
		var childNode = this.Children.FirstOrDefault(child => child.Name == name) ?? this.CreateChild(name);
		return childNode;
	}

	private NodeTypeInfo GetChildTypeInfo(string name)
	{
		// Get item if collection.
		if (this.TypeInfo.Type.GetInterfaces().Contains(typeof(IEnumerable)))
		{
			// Check if index provided.
			int itemIndex;
			if (Int32.TryParse(name, out itemIndex))
			{
				// Return item.
				return new EnumerableNode() { Type = this.TypeInfo.Type.GetElementType(), Index = itemIndex };
			}
		}

		// Get property.
		var reflectionProperty = ReflectionUtils.GetPublicProperty(this.TypeInfo.Type, name);
		if (reflectionProperty != null)
		{
			return new PropertyNode { Type = reflectionProperty.PropertyType, Property = reflectionProperty };
		}

		// Get field.
		var reflectionField = ReflectionUtils.GetPublicField(this.TypeInfo.Type, name);
		if (reflectionField != null)
		{
			return new FieldNode { Type = reflectionField.FieldType, Field = reflectionField };
		}

		// Get method.
		var reflectionMethod = ReflectionUtils.GetPublicMethod(this.TypeInfo.Type, name);
		if (reflectionMethod != null)
		{
			return new MethodNode { Type = reflectionMethod.ReturnType, Method = reflectionMethod };
		}

		return null;
	}            

	private IEnumerator WaitUpdateContent()
	{
		// Get object of the node.
		yield return new Routine(this.Value.WaitSet(this.TypeInfo.GetValue(this.ParentObject)));
	}

	#endregion

	private class FieldNode : NodeTypeInfo
	{
		#region Properties

		public FieldInfo Field { private get; set; }

		#endregion

		#region Public Methods and Operators

		public override object GetValue(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			// Get field value.
			if (this.Field != null)
			{
				return this.Field.GetValue(obj);
			}

			return null;
		}

		public override void SetValue(object obj, object value)
		{
			if (obj == null)
			{
				return;
			}

			if (this.Field == null)
			{
				return;
			}

			// Set field value.
			this.Field.SetValue(obj, value);
		}

		#endregion
	}

	private class PropertyNode : NodeTypeInfo
	{
		#region Properties

		public PropertyInfo Property { private get; set; }

		#endregion

		#region Public Methods and Operators

		public override object GetValue(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			// Get property value.
			if (this.Property != null)
			{
				return this.Property.GetValue(obj, null);
			}

			return null;
		}

		public override void SetValue(object obj, object value)
		{
			if (obj == null)
			{
				return;
			}

			if (this.Property == null)
			{
				return;
			}

			// Set property value.
			if (this.Property.CanWrite)
			{
				this.Property.SetValue(obj, value, null);
			}
			else
			{
				throw new InvalidOperationException("Property '" + this.Property.Name + "' is read-only.");
			}
		}

		#endregion
	}

	private class MethodNode : NodeTypeInfo
	{
		#region Properties

		public MethodInfo Method { private get; set; }

		#endregion

		#region Public Methods and Operators

		public override object GetValue(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			// Get delegate.
			if (this.Method != null)
			{
				var args = new List<Type>(this.Method.GetParameters().Select(p => p.ParameterType));
				var delegateType = Expression.GetFuncType(args.Concat(new Type[]{ Method.ReturnType }).ToArray());
				return ReflectionUtils.CreateDelegate(delegateType, obj, this.Method);
			}

			return null;
		}

		#endregion
	}

	private class NodeTypeInfo
	{
		#region Properties

		public Type Type { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///   Returns the value using the type info on the specified object.
		/// </summary>
		/// <param name="obj">Object to use the type info on.</param>
		/// <returns>Current value when using type info on specified object.</returns>
		public virtual object GetValue(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual void SetValue(object obj, object value)
		{
			throw new InvalidOperationException("Data node of type '" + this.Type + "' is read-only.");
		}

		#endregion
	}

	private class EnumerableNode : NodeTypeInfo
	{
		#region Properties

		public int Index { get; set; }

		#endregion

		#region Public Methods and Operators

		public override object GetValue(object obj)
		{
			// Check if enumerable.
			var enumerable = obj as IEnumerable;
			if (enumerable == null)
			{
				return null;
			}

			var index = 0;
			foreach (var item in enumerable)
			{
				if (index == this.Index)
				{
					return item;
				}
				++index;
			}

			return null;
		}

		#endregion
	}
}

public class MessageContext
{
	#region Constants

	public const char PathSeparator = '.';

	#endregion

	#region Fields

	/// <summary>
	///   Root data node.
	/// </summary>
	private readonly MessageDataNode root;

	#endregion

	#region Constructors and Destructors

	/// <summary>
	///   Constructor.
	/// </summary>
	protected MessageContext()
	{
		this.root = new MessageDataNode(this);
	}

	#endregion

	#region Public Methods and Operators

	/// <summary>
	///   Registers a callback at the specified path of the context.
	/// </summary>
	/// <param name="path">Path to register for.</param>
	/// <param name="onValueChanged">Callback to invoke when value at the specified path changed.</param>
	/// <exception cref="ArgumentException">Thrown if path is invalid for this context.</exception>
	/// <returns>Current value at specified path.</returns>
	public object RegisterListener(string path, Message<object>.Handler waitOnValueChanged)
	{
		var node = this.root.FindDescendant(path);
		if (node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		// Register for value change.
		node.Value.Changed.AddHandler(waitOnValueChanged);

		return node.Value;
	}

	/// <summary>
	///   Removes the callback from the specified path of the context.
	/// </summary>
	/// <param name="path">Path to remove callback from.</param>
	/// <param name="onValueChanged">Callback to remove.</param>
	/// <exception cref="ArgumentException">Thrown if path is invalid for this context.</exception>
	public void RemoveListener(string path, Message<object>.Handler waitOnValueChanged)
	{
		var node = this.root.FindDescendant(path);
		if (node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		// Remove from value change.
		node.Value.Changed.RemoveHandler(waitOnValueChanged);
	}

	/// <summary>
	///   Sets the specified value at the specified path.
	/// </summary>
	/// <param name="path">Path to set the data value at.</param>
	/// <exception cref="ArgumentException">Thrown if path is invalid for this context.</exception>
	/// <exception cref="InvalidOperationException">Thrown if data at specified path can't be changed.</exception>
	/// <param name="value">Value to set.</param>
	public IEnumerator WaitSetValue(string path, object value)
	{
		var node = this.root.FindDescendant(path);
		if (node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		yield return new Routine(node.WaitSetValue(value));
	}

	#endregion
}



