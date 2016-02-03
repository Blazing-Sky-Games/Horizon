using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public abstract class MainContextBase : Context
{
	private bool isLoaded;
	private bool launched;

	private Coroutine loading;
	private Coroutine launching;

	public Coroutine LoadIfNotLoaded()
	{
		if(!isLoaded)
		{
			ServiceLocator.RegisterService<ICoroutineService,CoroutineService>();
			loading = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(WaitLoad());
			loading.Step(null);
		}
		return loading;
	}

	//TODO fix this HACK
	protected bool shouldLaunchIfCore;

	public Coroutine LaunchIfNotLaunched()
	{
		if(!launched && loading != null && loading.Done)
		{
			ServiceLocator.RegisterService<ICoroutineService,CoroutineService>();
			launching = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(WaitLaunch());
			launching.Step(null);
		}
		else
		{
			shouldLaunchIfCore = true;
		}

		return launching;
	}

	protected virtual IEnumerator WaitLaunch()
	{
		launched = true;
		yield break;
	}
		
	protected virtual IEnumerator WaitLoad ()
	{ 
		isLoaded = true;
		yield break;
	}

	public virtual void Update ()
	{
	}

	public virtual void Unload ()
	{
		launched = false;
		launching = null;
		isLoaded = false;
		loading = null;
	}
}