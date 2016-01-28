using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ContextObjectLoader : MonoBehaviour
{
	public List<MainContextHolder> mainContextHolders;

	void Awake()
	{
		ServiceLocator.GetService<IContextLoadingService>().ContextLoading.AddAction(OnContextLoading);
		ServiceLocator.GetService<IContextLoadingService>().ContextUnloading.AddAction(OnContextUnloading);

		foreach(MainContextHolder holder in mainContextHolders)
		{
			holder.gameObject.SetActive(false);
		}
	}

	public void OnContextLoading (MainContextBase context)
	{
		ServiceLocator.GetService<ILoggingService>().Log("activating context object");

		MainContextHolder holder = mainContextHolders.Where(x => x.ContextType == context.GetType()).FirstOrDefault();
		if(holder != null)
		{
			holder.Context = context;
		}

		holder.gameObject.SetActive(true);
	}

	public void OnContextUnloading (MainContextBase context)
	{
		ServiceLocator.GetService<ILoggingService>().Log("deactivating context object");

		MainContextHolder holder = mainContextHolders.Where(x => x.ContextType == context.GetType()).FirstOrDefault();
		if(holder != null)
		{
			holder.Context = null;
		}

		holder.gameObject.SetActive(false);
	}
}
