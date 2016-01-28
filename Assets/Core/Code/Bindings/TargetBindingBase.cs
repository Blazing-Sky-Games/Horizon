using System;
using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Core.Utils;
using UnityEngine;

public abstract class TargetBindingBase : MonoBehaviour
{
	public UnityEngine.Object Target;

	private DataNode root;

	protected virtual void Awake ()
	{
		root = new DataNode(Target);
	}

	public object RegisterListener (string path, Action<object> onValueChanged)
	{
		var node = this.root.FindDescendant(path);
		if(node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		// Register for value change.
		node.ValueChanged += onValueChanged;

		return node.Value;
	}

	public void RemoveListener (string path, Action<object> onValueChanged)
	{
		var node = this.root.FindDescendant(path);
		if(node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		// Remove from value change.
		node.ValueChanged -= onValueChanged;
	}

	public void SetValue (string path, object value)
	{
		var node = this.root.FindDescendant(path);
		if(node == null)
		{
			throw new ArgumentException("Invalid path '" + path + "' for type " + this.GetType(), "path");
		}

		node.SetValue(value);
	}
}