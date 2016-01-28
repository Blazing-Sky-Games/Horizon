using System;
using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Core.Utils;
using UnityEngine;

public class TargetPathAttribute : PropertyAttribute
{
	public TargetPathAttribute()
	{
		this.Filter = ContextMemberFilter.All;
	}

	public ContextMemberFilter Filter { get; set; }

	public Type NodeType { get; set; }

	public string PathDisplayName { get; set; }
}
