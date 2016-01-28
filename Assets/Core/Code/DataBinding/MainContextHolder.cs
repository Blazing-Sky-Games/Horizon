using System;
using Slash.Unity.DataBind.Core.Presentation;

public class MainContextHolder : ContextHolder
{
	protected override void OnContextChanged ()
	{
		base.OnContextChanged();

		(Context as MainContextBase).LoadIfNotLoaded();
	}

	void Update()
	{
		(Context as MainContextBase).Update();
	}

	void OnDestroy()
	{
		(Context as MainContextBase).Unload();
	}
}


