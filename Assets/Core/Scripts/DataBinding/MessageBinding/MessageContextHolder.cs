using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections;

public class MessageContextHolder : MonoBehaviour
{
	[TypeSelection(BaseType = typeof(MessageContext))]
	public string contextType;

	public bool createContext;

	public MessageProperty Context;

	public Type ContextType
	{
		get
		{
			try
			{
				return this.contextType != null ? ReflectionUtils.FindType(this.contextType) : null;
			}
			catch (TypeLoadException)
			{
				Debug.LogError("Can't find context type '" + this.contextType + "'.", this);
				return null;
			}
		}
		set
		{
			this.contextType = value != null ? value.AssemblyQualifiedName : null;
		}
	}

	protected virtual void Awake()
	{
		if(this.Context == null && this.ContextType != null && this.createContext)
		{
			this.Context = new MessageProperty(Activator.CreateInstance(this.ContextType));
		}
		else
		{
			this.Context = new MessageProperty();
		}

		this.Context.Changed.AddHandler(WaitOnContextChanged);
	}

	protected virtual IEnumerator WaitOnContextChanged(object value)
	{
		// Update child bindings as context changed.
		var uiBindings = this.gameObject.GetComponentsInChildren<Setter>(true);
		foreach (var binding in uiBindings)
		{
			binding.OnContextChanged();
		}
		var providers = this.gameObject.GetComponentsInChildren<DataProvider>(true);
		foreach (var provider in providers)
		{
			provider.OnContextChanged();
		}
		var commands = this.gameObject.GetComponentsInChildren<Command>(true);
		foreach (var command in commands)
		{
			command.OnContextChanged();
		}

		// Update child message bindings as context changed.
		var messageUiBindings = this.gameObject.GetComponentsInChildren<MessageSetter>(true);
		foreach (var binding in messageUiBindings)
		{
			yield return new Routine(binding.WaitOnContextChanged());
		}
		var messageProviders = this.gameObject.GetComponentsInChildren<MessageDataProvider>(true);
		foreach (var provider in messageProviders)
		{
			yield return new Routine(provider.WaitOnContextChanged());
		}
		var eventCommands = this.gameObject.GetComponentsInChildren<EventCommand>(true);
		foreach (var command in eventCommands)
		{
			yield return new Routine(command.WaitOnContextChanged());
		}
	}
}


