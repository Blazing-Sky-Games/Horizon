using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenMainView : DataFromEditorView<EmptyLogic,EmptyData,EmptyData>
{
	public Image LoadingIcon;

	public float rotateSpeed = -90;

	protected override IEnumerator MainRoutine ()
	{
		while(true)
		{
			LoadingIcon.transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * rotateSpeed);
			yield return 0;
		}
	}
}
