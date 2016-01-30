using System;
using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Core.Utils;
using UnityEngine;

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