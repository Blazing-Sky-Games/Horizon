using System;
using System.Collections;
using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Utils;
using Slash.Unity.DataBind.Core.Utils;

[UnityEngine.AddComponentMenu("Data Bind/UnityUI/Commands/[DB] Message Command (Horizon)")]
public class MessageCommand : TargetBindingBase
{
	[ContextPath(Filter = ContextMemberFilter.Methods | ContextMemberFilter.Recursive, PathDisplayName = "Handler Path")]
	public string contextPath;
	private ContextNode contextNode;
	private Delegate command;

	[TargetPathAttribute(Filter = ContextMemberFilter.Fields, NodeType = typeof(IGenericMessage), PathDisplayName = "Message Path")]
	public string targetPath;
	private TargetNode targetNode;
	private IGenericMessage msg;

	protected override void Awake ()
	{
		base.Awake();
		contextNode = new ContextNode(ContextTypeUtils.GetContextObject(this), contextPath);
		targetNode = new TargetNode(this, targetPath);

		command = contextNode.SetValueListener(OnCommandChanged) as Delegate;
		msg = targetNode.SetValueListener(OnMsgChanged) as IGenericMessage;
	}

	void OnMsgChanged (object obj)
	{
		RemoveHandler();
		msg = (Message)obj;
		RegisterHandler();
	}

	protected virtual void OnEnable ()
	{
		if(msg == null)
		{
			return;
		}

		RegisterHandler();
	}

	protected virtual void OnDisable ()
	{
		if(msg == null)
		{
			return;
		}

		RemoveHandler();
	}

	protected IEnumerator OnMessage (object[] args)
	{
		return InvokeCommand(args);
	}

	IEnumerator InvokeCommand (object[] args)
	{
		return this.command.DynamicInvoke(args) as IEnumerator;
	}

	protected virtual void RegisterHandler ()
	{
		msg.AddHandlerGeneric(OnMessage);
	}

	protected virtual void RemoveHandler ()
	{
		msg.RemoveHandlerGeneric(OnMessage);
	}

	private void OnCommandChanged (object obj)
	{
		this.command = obj as Delegate;
	}
}