using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

public abstract class EventCommand : MonoBehaviour
{
	public DataProvider[] AdditionalArguments;
	public MessageDataProvider[] AdditionalMessageArguments;

	//  Path of event queue to send events to.
	[ContextPath]
	public string Path;

	private GenericEventQueue command;

	private MessageContextNode node;

	///   sends event to queue
	public void InvokeCommand()
	{
		this.InvokeCommand(new object[]{});
	}
		
	///   sends event to bound queue with the specified arguments.
	public void InvokeCommand(params object[] args)
	{
		if (this.command == null)
		{
			return;
		}

		// Add additional arguments if there are any.
		var commandArgs = args;
		if (this.AdditionalArguments != null && this.AdditionalArguments.Length > 0)
		{
			var argList = new List<object>();
			argList.AddRange(args);
			argList.AddRange(
				this.AdditionalArguments.Select(
					additionArgument => additionArgument != null ? additionArgument.Value : null));
			commandArgs = argList.ToArray();
		}

		if (this.AdditionalMessageArguments != null && this.AdditionalMessageArguments.Length > 0)
		{
			var argList = new List<object>();
			argList.AddRange(commandArgs);
			argList.AddRange(
				this.AdditionalMessageArguments.Select(
					additionArgument => additionArgument != null ? additionArgument.Value.ObjectValue : null));
			commandArgs = argList.ToArray();
		}
			
		// send the event.
		this.command.DynamicSend(commandArgs);
	}
		
	//   Has to be called when an anchestor context changed as the data value may change.
	public IEnumerator WaitOnContextChanged()
	{
		if (this.node != null)
		{
			yield return new Routine(this.node.WaitOnHierarchyChanged());
		}
	}

	protected virtual void Awake()
	{
		this.node = new MessageContextNode(this.gameObject, this.Path);
	}

	protected void OnDestroy()
	{
		this.node.SetValueListener(null);
	}

	protected virtual void Start()
	{
		// Monitor command.
		this.command = this.node.SetValueListener(this.OnCommandChanged) as GenericEventQueue;
	}

	private IEnumerator OnCommandChanged(object obj)
	{
		this.command = obj as GenericEventQueue;
		yield break;
	}
}



