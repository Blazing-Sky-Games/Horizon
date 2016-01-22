using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LoadingScreenMainView : DataFromEditorView<EmptyLogic,EmptyData,EmptyData>
{
	public Image LoadingIcon;

	public float rotateSpeed = -90;

	protected override void SetUp ()
	{
		base.SetUp();
		logManager = ServiceUtility.GetServiceReference<LogManager>();
	}

	protected override IEnumerator MainRoutine ()
	{
		while(IsAlive)
		{
			try{
			LoadingIcon.transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * rotateSpeed);
			}
			catch(Exception e)
			{
				logManager.Dereference().CoreLogFile.Log(e.ToString());
			}
			yield return 0;
		}
	}

	WeakReference<LogManager> logManager;
}
