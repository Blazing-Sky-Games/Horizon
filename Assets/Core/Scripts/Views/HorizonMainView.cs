using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HorizonMainView : DataFromEditorView<EmptyLogic,EmptyData,EmptyData>
{
	private void Update()
	{
		Horizon.Core.Logic.Globals.Coroutines.UpdateCoroutines();
	}

	protected override IEnumerator MainRoutine ()
	{
		yield return new Routine(LoadScene(SceneType.LoadingScreen));
		yield return new Routine(LoadScene(SceneType.Combat));
		yield return new Routine(UnloadScene(SceneType.LoadingScreen));
	}

	public IEnumerator GoToSceneThroughLoadingScreen(SceneType scene)
	{
		yield return new Routine(UnloadScene(currentScene));
		yield return new Routine(LoadScene(SceneType.LoadingScreen));
		yield return new Routine(LoadScene(scene));
		yield return new Routine(UnloadScene(SceneType.LoadingScreen));
	}

	public IEnumerator LoadScene(SceneType scene)
	{
		Horizon.Core.Views.Globals.LoadingScene = true;
		var asop = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

		while(!asop.isDone)
		{
			yield return 0;
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));

		currentScene = scene;
		Horizon.Core.Views.Globals.LoadingScene = false;
	}

	public IEnumerator UnloadScene(SceneType scene)
	{
		SceneManager.UnloadScene(scene.ToString());
		var asop = Resources.UnloadUnusedAssets();
		while(!asop.isDone)
		{
			yield return 0;
		}
	}

	private SceneType currentScene;
}
