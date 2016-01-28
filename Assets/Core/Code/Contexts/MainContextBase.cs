using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public abstract class MainContextBase : Context
{
	private bool isLoaded;

	public Coroutine LoadIfNotLoaded()
	{
		if(!isLoaded)
		{
			return Load();
		}
		else
		{
			return null;
		}
	}
		
	protected virtual Coroutine Load ()
	{ 
		isLoaded = true;
		return null;
	}

	public virtual void Update ()
	{
	}

	public virtual void Unload ()
	{
		isLoaded = false;
	}
}