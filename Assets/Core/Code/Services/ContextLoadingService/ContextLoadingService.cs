using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Core.Scripts.Contexts;


public class ContextLoadingService : IContextLoadingService
{
	#region IContextLoadingService implementation
	public IEnumerator LoadContext<ContextType> () where ContextType : MainContextBase
	{
		throw new System.NotImplementedException();
	}
	public IEnumerator LoadContext (System.Type contextType)
	{
		throw new System.NotImplementedException();
	}
	public IEnumerator UnloadContext<ContextType> () where ContextType : MainContextBase
	{
		throw new System.NotImplementedException();
	}
	public IEnumerator UnloadContext (System.Type contextType)
	{
		throw new System.NotImplementedException();
	}
	public System.Type LoadingContextType
	{
		get
		{
			throw new System.NotImplementedException();
		}
		set
		{
			throw new System.NotImplementedException();
		}
	}
	public Message<MainContextBase> ContextLoaded
	{
		get
		{
			throw new System.NotImplementedException();
		}
	}
	public Message<MainContextBase> ContextUnloaded
	{
		get
		{
			throw new System.NotImplementedException();
		}
	}
	#endregion
	#region IService implementation
	public void LoadService ()
	{
		throw new System.NotImplementedException();
	}
	public void UnloadService ()
	{
		throw new System.NotImplementedException();
	}
	#endregion
	
}