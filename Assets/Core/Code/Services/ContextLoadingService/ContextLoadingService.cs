using System.Collections;
using System;

public class ContextLoadingService : IContextLoadingService
{
	#region IContextLoadingService implementation
	public IEnumerator LoadContext<ContextType> () where ContextType : MainContextBase
	{
		throw new NotImplementedException();
	}
	public IEnumerator LoadContext (System.Type contextType)
	{
		throw new NotImplementedException();
	}
	public IEnumerator UnloadContext<ContextType> () where ContextType : MainContextBase
	{
		throw new NotImplementedException();
	}
	public IEnumerator UnloadContext (System.Type contextType)
	{
		throw new NotImplementedException();
	}
	public System.Type LoadingContextType
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}
	public Message<MainContextBase> ContextLoaded
	{
		get
		{
			throw new NotImplementedException();
		}
	}
	public Message<MainContextBase> ContextUnloaded
	{
		get
		{
			throw new NotImplementedException();
		}
	}
	#endregion
	#region IService implementation
	public void LoadService ()
	{
		throw new NotImplementedException();
	}
	public void UnloadService ()
	{
		throw new NotImplementedException();
	}
	#endregion
	
}