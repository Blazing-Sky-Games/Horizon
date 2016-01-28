using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class ContextLoadingService : Service, IContextLoadingService
{
	public IEnumerator WaitLoadContext<ContextType> () where ContextType : MainContextBase
	{
		yield return new Routine(WaitLoadContext(typeof(ContextType)));
	}

	public IEnumerator WaitLoadContext (Type contextType)
	{
		if(IsLoaded(contextType))
			yield break;

		MainContextBase context = (MainContextBase)Activator.CreateInstance(contextType);
		yield return new Routine(contextLoading.WaitSend(context));
		yield return new Routine(context.WaitLoad());
		yield return new Routine(ContextLoaded.WaitSend(context));
		loadedContexts.Add(context);
	}

	public IEnumerator WaitUnloadContext<ContextType> () where ContextType : MainContextBase
	{
		yield return new Routine(WaitUnloadContext(typeof(ContextType)));
	}

	public IEnumerator WaitUnloadContext (Type contextType)
	{
		if(!IsLoaded(contextType))
			yield break;

		MainContextBase context = loadedContexts.First(x => x.GetType() == contextType);
		yield return new Routine(contextUnloading.WaitSend(context));
		context.Unload();
		yield return new Routine(ContextUnloaded.WaitSend(context));

		loadedContexts.Remove(context);
	}

	public bool IsLoaded<ContextType> () where ContextType : MainContextBase
	{
		return IsLoaded(typeof(ContextType));
	}

	public bool IsLoaded (Type contextType)
	{
		return loadedContexts.Any(x => x.GetType() == contextType);
	}

	public Type LoadingContextType
	{
		get{ return loadingContextType; }
		set{ loadingContextType = value; }
	}

	public Message<MainContextBase> ContextLoading
	{
		get{ return contextLoading; }
	}

	public Message<MainContextBase> ContextLoaded
	{
		get{ return contextLoaded; }
	}

	public Message<MainContextBase> ContextUnloading
	{
		get{ return contextUnloading; }
	}

	public Message<MainContextBase> ContextUnloaded
	{
		get{ return contextUnloaded; }
	}

	private Type loadingContextType;

	private readonly Message<MainContextBase> contextLoading = new Message<MainContextBase>();
	private readonly Message<MainContextBase> contextLoaded = new Message<MainContextBase>();
	private readonly Message<MainContextBase> contextUnloading = new Message<MainContextBase>();
	private readonly Message<MainContextBase> contextUnloaded = new Message<MainContextBase>();

	private readonly HashSet<MainContextBase> loadedContexts = new HashSet<MainContextBase>();
}