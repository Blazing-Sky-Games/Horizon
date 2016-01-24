using UnityEngine;
using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public class CompContext : Context 
{
	public IEnumerator logComputation(int a, int b)
	{
		Debug.Log(a + b);

		int frames = 0;
		while(frames < 100)
		{
			frames++;
			yield return 0;
		}
	}

}
