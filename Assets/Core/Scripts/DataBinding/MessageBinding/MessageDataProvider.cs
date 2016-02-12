using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public abstract class MessageDataProvider : MonoBehaviour
{
	///   Child bindings.
	private readonly List<DataBinding> bindings = new List<DataBinding>();
	private readonly List<MessageDataBinding> messageBindings = new List<MessageDataBinding>();
	private bool isMonitoringBindings;

	public bool IsInitialized
	{
		get
		{
			return (this.bindings == null || this.bindings.All(binding => binding.IsInitialized)) && (this.messageBindings == null || this.messageBindings.All(binding => binding.IsInitialized));
		}
	}

	public MessageProperty Value;

	public virtual IEnumerator WaitOnContextChanged()
	{
		foreach (var binding in this.bindings)
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

		if (this.isMonitoringBindings)
		{
			binding.ValueChanged += this.OnBindingValueChanged;
		}

		this.bindings.Add(binding);
	}

	protected void AddMessageBinding(MessageDataBinding binding)
	{
		// Init.
		binding.Init(this.gameObject);

		if (this.isMonitoringBindings)
		{
			binding.Value.Changed.AddHandler(this.WaitOnMessageBindingValueChanged);
		}

		this.messageBindings.Add(binding);
	}

	protected virtual void OnDisable()
	{
		this.UnregisterFromValueChanges();
	}

	protected virtual void OnEnable()
	{
		this.RegisterForValueChanges();
		var bindingsInitialized = this.bindings.All(binding => binding.IsInitialized) && this.messageBindings.All(binding => binding.IsInitialized);
		if (bindingsInitialized)
		{
			OnBindingValueChanged(null);
		}
	}

	protected void RemoveBinding(DataBinding binding)
	{
		if (this.isMonitoringBindings)
		{
			binding.ValueChanged -= this.OnBindingValueChanged;
		}

		// Deinit.
		binding.Deinit();

		this.bindings.Remove(binding);
	}

	protected void RemoveMessageBinding(MessageDataBinding binding)
	{
		if (this.isMonitoringBindings)
		{
			binding.Value.Changing.RemoveHandler(this.WaitOnMessageBindingValueChanged);
		}

		// Deinit.
		binding.Deinit();

		this.messageBindings.Remove(binding);
	}

	protected virtual void Start()
	{
		OnBindingValueChanged(null);
	}
		
	///   Called when the value of the data provider should be updated.
	protected abstract IEnumerator WaitUpdateValue();

	private void OnBindingValueChanged(object newValue)
	{
		ServiceLocator.GetService<CoroutineService>().StartCoroutine(WaitUpdateValue());
	}

	private IEnumerator WaitOnMessageBindingValueChanged(object value)
	{
		yield return new Routine(WaitUpdateValue());
	}

	private void RegisterForValueChanges()
	{
		foreach (var binding in this.bindings)
		{
			binding.ValueChanged += this.OnBindingValueChanged;
		}
		foreach (var binding in this.messageBindings)
		{
			binding.Value.Changing.AddHandler(this.WaitOnMessageBindingValueChanged);
		}
		this.isMonitoringBindings = true;
	}

	private void UnregisterFromValueChanges()
	{
		foreach (var binding in this.bindings)
		{
			binding.ValueChanged -= this.OnBindingValueChanged;
		}
		foreach (var binding in this.messageBindings)
		{
			binding.Value.Changing.RemoveHandler(this.WaitOnMessageBindingValueChanged);
		}
		this.isMonitoringBindings = false;
	}
}



