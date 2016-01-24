using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;


public static class SceneUtility
{
	public static readonly Message<SceneType> SceneLoaded = new Message<SceneType>();
	public static readonly Message<SceneType> SceneUnloaded = new Message<SceneType>();

	public static IEnumerator GoToSceneThroughLoadingScreen (SceneType scene)
	{
		yield return new Routine(UnloadScene(currentScene));
		yield return new Routine(LoadScene(SceneType.LoadingScreen));
		yield return new Routine(LoadScene(scene));
		yield return new Routine(UnloadScene(SceneType.LoadingScreen));
	}

	public static IEnumerator LoadScene (SceneType scene)
	{
		LoadingScene = true;
		var asop = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

		while(!asop.isDone)
		{
			yield return new WaitForNextFrame();
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));

		currentScene = scene;
		LoadingScene = false;
		yield return new Routine(SceneLoaded.WaitSend(scene));
	}

	public static IEnumerator UnloadScene (SceneType scene)
	{
		yield return new Routine(SceneUnloaded.WaitSend(scene));
		SceneManager.UnloadScene(scene.ToString());
		var asop = Resources.UnloadUnusedAssets();
		while(!asop.isDone)
		{
			yield return new WaitForNextFrame();
		}
	}
	public static bool LoadingScene;

	private static SceneType currentScene;
}