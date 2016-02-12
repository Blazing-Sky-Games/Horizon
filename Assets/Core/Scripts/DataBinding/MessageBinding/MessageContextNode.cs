using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;

using Object = UnityEngine.Object;
using System.Collections;
using System.Linq;
using Slash.Unity.DataBind.Core.Data;

public class MessageContextNode
{
	#region Constants

	private const int MaxPathDepth = 100500;

	#endregion

	#region Fields

	/// <summary>
	///   Context cache for faster look up.
	/// </summary>
	private readonly Dictionary<int, MessageContextHolder> contexts = new Dictionary<int, MessageContextHolder>();

	/// <summary>
	///   Game object to do the lookup for.
	/// </summary>
	private readonly GameObject gameObject;

	/// <summary>
	///   Master path cache for faster look up.
	/// </summary>
	private readonly Dictionary<int, string> masterPaths = new Dictionary<int, string>();

	/// <summary>
	///   Path in context this node is bound to.
	/// </summary>
	private readonly string path;

	/// <summary>
	///   Context to use for data lookup.
	/// </summary>
	private object context;

	/// <summary>
	///   Full path to data starting from context.
	/// </summary>
	private string contextPath;

	/// <summary>
	///   Callback when value changed.
	/// </summary>
	private Message<object>.Handler valueChanged;

	/// <summary>
	///   Constructor.
	/// </summary>
	/// <param name="gameObject">Game object this node is assigned to.</param>
	/// <param name="path">Path in context this node is bound to.</param>
	public MessageContextNode(GameObject gameObject, string path)
	{
		Context = new MessageProperty();
		Context.Changing.AddHandler(WaitContextChanging);

		this.gameObject = gameObject;
		this.path = path;
		ServiceLocator.GetService<ICoroutineService>().StartCoroutine(this.WaitOnHierarchyChanged());
	}

	MessageProperty Context;

	IEnumerator WaitContextChanging(object value)
	{
		this.RemoveListener();
		
		this.context = value;
		
		// Add listener to new context.
		var initialValue = this.RegisterListener();
		if (this.valueChanged != null)
		{
			yield return new Routine(this.valueChanged(initialValue));
		}
	}

	/// <summary>
	///   Indicates if the context node already holds a valid value.
	/// </summary>
	public bool IsInitialized
	{
		get
		{
			return this.context != null;
		}
	}

	#endregion

	#region Public Methods and Operators

	/// <summary>
	///   Informs the context node that the hierarchy changed, so the context and/or master paths may have changed.
	///   Has to be called:
	///   - Anchestor context changed.
	///   - Anchestor master path changed.
	/// </summary>
	public IEnumerator WaitOnHierarchyChanged()
	{
		// Update master paths.
		this.UpdateCache();

		// Update context.
		var depthToGo = GetPathDepth(this.path);

		// Take first context holder which is deep enough to use as a starting point.
		var contextHolderPair = this.contexts.FirstOrDefault(pair => depthToGo <= pair.Key);
		var contextHolder = contextHolderPair.Value;

		object newContext = contextHolder != null ? contextHolder.Context : null;

		// Adjust full path.
		this.contextPath = this.GetFullCleanPath(depthToGo, contextHolderPair.Key);

		yield return new Routine(this.Context.WaitSet(newContext));
	}

	/// <summary>
	///   Sets the specified value at the specified path.
	/// </summary>
	/// <param name="value">Value to set.</param>
	public IEnumerator WaitSetValue(object value)
	{
		// Set value on data context.
		var dataContext = this.context as MessageContext;
		if (dataContext == null)
		{
			yield break;
		}

		yield return new Routine(dataContext.WaitSetValue(this.contextPath, value));
	}

	/// <summary>
	///   Sets the callback which is called when the value of the monitored data in the context changed.
	/// </summary>
	/// <param name="onValueChanged">Callback to invoke when the value of the monitored data in the context changed.</param>
	/// <returns>Initial value.</returns>
	public object SetValueListener(Message<object>.Handler onValueChanged)
	{
		// Remove old callback.
		this.RemoveListener();

		this.valueChanged = onValueChanged;

		// Add new callback.
		return this.RegisterListener();
	}

	#endregion

	#region Methods

	private static string GetCleanPath(string path)
	{
		if (!path.StartsWith("#"))
		{
			return path;
		}
		var dotIndex = path.IndexOf(MessageContext.PathSeparator);
		var result = (dotIndex < 0) ? null : path.Substring(dotIndex + 1);
		return result;
	}

	/// <summary>
	///   Converts the specified path to a full, clean path. I.e. replaces the depth value and prepends the master paths.
	/// </summary>
	/// <returns>Full clean path for the specified path.</returns>
	private string GetFullCleanPath(int startDepth, int endDepth)
	{
		var cleanPath = GetCleanPath(this.path);

		var fullPath = cleanPath;
		for (var depth = startDepth; depth < endDepth; ++depth)
		{
			string masterPath;
			if (this.masterPaths.TryGetValue(depth, out masterPath) && !string.IsNullOrEmpty(masterPath))
			{
				fullPath = masterPath + MessageContext.PathSeparator + fullPath;
			}
		}

		return fullPath;
	}

	private static int GetPathDepth(string path)
	{
		if (!path.StartsWith("#"))
		{
			return 0;
		}
		var depthString = path.Substring(1);
		var dotIndex = depthString.IndexOf(MessageContext.PathSeparator);
		if (dotIndex >= 0)
		{
			depthString = depthString.Substring(0, dotIndex);
		}
		if (depthString == "#")
		{
			return MaxPathDepth;
		}
		int depth;
		if (int.TryParse(depthString, out depth))
		{
			return depth;
		}
		Debug.LogWarning("Failed to get binding context depth for: " + path);
		return 0;
	}

	/// <summary>
	///   Registers a callback at the current context.
	/// </summary>
	/// <returns>Current value.</returns>
	private object RegisterListener()
	{
		// Return context itself if no path set.
		if (string.IsNullOrEmpty(this.contextPath))
		{
			return this.context;
		}

		var dataContext = this.context as MessageContext;
		if (dataContext != null)
		{
			try
			{
				return this.valueChanged != null
					? dataContext.RegisterListener(this.contextPath, WaitValueChanged)
						: null;
			}
			catch (ArgumentException e)
			{
				Debug.LogError(e, this.gameObject);
				return null;
			}
		}

		// If context is not null, but path is set, it is not derived from context class, so 
		// the path can't be resolved. Log an error to inform the user that the context should be derived
		// from the Context class.
		if (this.context != null)
		{
			Debug.LogError(
				string.Format(
					"Context of type '{0}' is not derived from '{1}', but path is set to '{2}'. Not able to get data from a non-context type.",
					this.context.GetType(),
					typeof(Context),
					this.contextPath),
				this.gameObject);
		}

		return null;
	}

	IEnumerator WaitValueChanged(object value)
	{
		yield return new Routine(this.valueChanged(value));
	}

	/// <summary>
	///   Removes the callback from the current context.
	/// </summary>
	private void RemoveListener()
	{
		// Return if no path set.
		if (string.IsNullOrEmpty(this.contextPath))
		{
			return;
		}

		var dataContext = this.context as MessageContext;
		if (dataContext == null || this.valueChanged == null)
		{
			return;
		}

		// Remove listener.
		try
		{
			dataContext.RemoveListener(this.contextPath, WaitValueChanged);
		}
		catch (ArgumentException e)
		{
			Debug.LogError(e, this.gameObject);
		}
	}

	/// <summary>
	///   Updates the master path and context cache.
	/// </summary>
	private void UpdateCache()
	{
		// Clear cache.
		this.contexts.Clear();
		this.masterPaths.Clear();

		var p = this.gameObject;

		var depth = 0;

		while (p != null)
		{
			var contextHolder = p.GetComponent<MessageContextHolder>();
			if (contextHolder != null)
			{
				this.contexts.Add(depth, contextHolder);
				++depth;
			}

			var masterPath = p.GetComponent<MasterPath>();
			if (masterPath != null)
			{
				// Process path.
				string pathRest = masterPath.Path;
				while (!string.IsNullOrEmpty(pathRest))
				{
					var separatorIndex = pathRest.LastIndexOf(MessageContext.PathSeparator);
					string pathSection;
					if (separatorIndex >= 0)
					{
						pathSection = pathRest.Substring(separatorIndex + 1);
						pathRest = pathRest.Substring(0, separatorIndex);
					}
					else
					{
						pathSection = pathRest;
						pathRest = null;
					}
					this.masterPaths.Add(depth, pathSection);
					++depth;
				}
			}
			p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
		}
	}

	#endregion

}





