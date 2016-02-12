using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;
using System.Collections;

public abstract class MessageSetter : MonoBehaviour
{
	private readonly List<DataBinding> bindings = new List<DataBinding>();
	private readonly List<MessageDataBinding> messageBindings = new List<MessageDataBinding>();

	public IEnumerator WaitOnContextChanged()
	{
		foreach(var binding in this.bindings)
		{
			binding.OnContextChanged();
		}

		foreach (var binding in this.messageBindings)
		{
			yield return new Routine(binding.WaitOnContextChanged());
		}
	}

	protected void AddBinding(DataBinding binding)
	{
		// Init.
		binding.Init(this.gameObject);

		this.bindings.Add(binding);
	}

	protected void RemoveBinding(DataBinding binding)
	{
		// Deinit.
		binding.Deinit();

		this.bindings.Remove(binding);
	}

	protected void AddMessageBinding(MessageDataBinding binding)
	{
		// Init.
		binding.Init(this.gameObject);

		this.messageBindings.Add(binding);
	}

	protected void RemoveMessageBinding(MessageDataBinding binding)
	{
		// Deinit.
		binding.Deinit();

		this.messageBindings.Remove(binding);
	}
}



