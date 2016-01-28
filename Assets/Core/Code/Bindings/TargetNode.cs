using System;

class TargetNode
{
	public TargetNode(TargetBindingBase tb, string path)
	{
		this.tb = tb;
		this.path = path;
	}

	public void SetValue (object value)
	{
		// Set value on data context.
		if(tb == null)
		{
			return;
		}

		try
		{
			tb.SetValue(path, value);
		}
		catch(ArgumentException e)
		{
			UnityEngine.Debug.LogError(e, tb);
		}
		catch(InvalidOperationException e)
		{
			UnityEngine.Debug.LogError(e, tb);
		}
	}

	public object SetValueListener (Action<object> onValueChanged)
	{
		// Remove old callback.
		this.RemoveListener();

		this.valueChangedCallback = onValueChanged;

		// Add new callback.
		return this.RegisterListener();
	}

	private object RegisterListener ()
	{
		// Return tb itself if no path set.
		if(string.IsNullOrEmpty(path))
		{
			return tb;
		}
				
		if(tb != null)
		{
			try
			{
				return this.valueChangedCallback != null
						? tb.RegisterListener(path, valueChangedCallback)
							: null;
			}
			catch(ArgumentException e)
			{
				UnityEngine.Debug.LogError(e, tb);
				return null;
			}
		}

		return null;
	}

	private void RemoveListener ()
	{
		// Return if no path set.
		if(string.IsNullOrEmpty(path))
		{
			return;
		}
				
		if(tb == null || valueChangedCallback == null)
		{
			return;
		}

		// Remove listener.
		try
		{
			tb.RemoveListener(path, valueChangedCallback);
		}
		catch(ArgumentException e)
		{
			UnityEngine.Debug.LogError(e, tb);
		}
	}

	private Action<object> valueChangedCallback;

	private readonly string path;
	private readonly TargetBindingBase tb;
}