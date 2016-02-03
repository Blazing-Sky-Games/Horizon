using System;
using Slash.Unity.DataBind.Core.Presentation;

public class MainContextHolder : ContextHolder
{
	private bool started;

	protected override void OnContextChanged ()
	{
		base.OnContextChanged();

		if(Context != null)
			(Context as MainContextBase).LoadIfNotLoaded();

		if(started && Context != null)
			(Context as MainContextBase).LaunchIfNotLaunched();
	}

	void Start()
	{
		if(Context != null)
			(Context as MainContextBase).LaunchIfNotLaunched();

		started = true;
	}

	void Update ()
	{
		if(Context != null)
			(Context as MainContextBase).Update();
	}

	void OnDestroy ()
	{
		if(Context != null)
			(Context as MainContextBase).Unload();
	}
}


