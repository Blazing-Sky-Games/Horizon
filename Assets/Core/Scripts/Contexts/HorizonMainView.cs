using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HorizonMainView : DataFromEditorView<EmptyLogic,EmptyData,EmptyData>
{
	protected override void Awake ()
	{
		ServiceUtility.Init();
		base.Awake();
	}

	protected override void SetUp ()
	{
		base.SetUp();
		logManager = ServiceUtility.GetServiceReference<LogManager>();
	}

	private void Update()
	{
		CoroutineUtility.UpdateCoroutines();
	}

	protected override IEnumerator MainRoutine ()
	{
		logManager.Dereference().CoreLogFileAndScreen.Log("MainRoutine");

		yield return new Routine(SceneUtility.LoadScene(SceneType.LoadingScreen));

		float dt = 0;
		while(dt < 4)
		{
			dt += Time.deltaTime;
			yield return new WaitForNextFrame();
		}

		yield return new Routine(SceneUtility.LoadScene(SceneType.Combat));
		yield return new Routine(SceneUtility.UnloadScene(SceneType.LoadingScreen));
	}

	WeakReference<LogManager> logManager;
}
