using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class ContextSceneLoader : MonoBehaviour
{
	private List<SceneRecord> scenes = new List<SceneRecord>();

	void Awake()
	{
		ServiceLocator.GetService<IContextLoadingService>().ContextLoading.AddHandler(WaitOnContextLoading);
		ServiceLocator.GetService<IContextLoadingService>().ContextUnloading.AddAction(OnContextUnloading);

		TextAsset sceneList = Resources.Load<TextAsset>("SceneList");

		StringReader sr = new StringReader(sceneList.text);
		string line = null;
		while((line = sr.ReadLine()) != null)
		{
			string[] elements = line.Split();
			scenes.Add(new SceneRecord(elements[0],elements[1],elements[2]));
		}
	}

	public IEnumerator WaitOnContextLoading (MainContextBase context)
	{
		var contextSceneRecord = scenes.Where(record => record.ContextType == context.GetType().Name).FirstOrDefault();

		if(contextSceneRecord != null)
		{
			var asop = SceneManager.LoadSceneAsync(contextSceneRecord.Path, LoadSceneMode.Additive);
			
			while(!asop.isDone)
			{
				yield return new WaitForNextUpdate();
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(contextSceneRecord.Path));

			MainContextHolder holder = GameObject.Find(contextSceneRecord.ContextGameObjectName).GetComponent<MainContextHolder>();
			holder.Context = context;
		}
	}

	public void OnContextUnloading (MainContextBase context)
	{
		var contextSceneRecord = scenes.Where(record => record.ContextType == context.GetType().Name).FirstOrDefault();

		if(contextSceneRecord != null)
		{
			MainContextHolder holder = GameObject.Find(contextSceneRecord.ContextGameObjectName).GetComponent<MainContextHolder>();
			holder.Context = null;

			SceneManager.UnloadScene(contextSceneRecord.Path);
		}
	}
}
