using UnityEngine;
using System.Collections;

public class coroutinetest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		CoroutineUtility.StartCoroutine(mainRot());
	}
	
	// Update is called once per frame
	void Update ()
	{
		CoroutineUtility.UpdateCoroutines();
	}

	IEnumerator mainRot()
	{
		yield return CoroutineUtility.StartCoroutine(subrot(0));
		yield return CoroutineUtility.StartCoroutine(subrot(1));

		Coroutine c1 = CoroutineUtility.StartCoroutine(subrot(2));
		Coroutine c2 = CoroutineUtility.StartCoroutine(subrot(3));

		yield return c1;
		yield return c2;

		yield return CoroutineUtility.StartCoroutine(subrot(4));
		yield return CoroutineUtility.StartCoroutine(subrot(4));
		yield return CoroutineUtility.StartCoroutine(subrot(6));
	}

	IEnumerator subrot(int x)
	{
		Debug.Log("start" + x.ToString());

		float elapsed = 0;
		while(elapsed < 4)
		{
			elapsed += Time.deltaTime;
			yield return new WaitForNextFrame();
		}

		Debug.Log("End" + x.ToString());
	}
}
