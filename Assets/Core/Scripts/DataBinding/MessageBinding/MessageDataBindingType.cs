using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;

using Object = UnityEngine.Object;
using System.Collections;

public enum MessageDataBindingType
{
	/// <summary>
	///   Data is fetched from a data context.
	/// </summary>
	Context,

	/// <summary>
	///   Data is taken from a specific provider.
	/// </summary>
	Provider,

	// data is taken from a message provider
	MessageProvider,

	/// <summary>
	///   Data is a constant value.
	/// </summary>
	Constant,

	/// <summary>
	///   Data is an object reference.
	/// </summary>
	Reference
}





