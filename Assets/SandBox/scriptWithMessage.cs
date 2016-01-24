using UnityEngine;
using System.Collections;

public class scriptWithMessage : MonoBehaviour 
{
	public Message<int, int> LogComputation = new Message<int, int>();

	public int a;
	public int b;

	void Start()
	{
		CoroutineUtility.StartCoroutine(MainRoutine());
	}

	void Update()
	{
		CoroutineUtility.UpdateCoroutines();
	}

	IEnumerator MainRoutine()
	{
		yield return new Routine(LogComputation.WaitSend(a,b));
		yield return new Routine(LogComputation.WaitSend(a,b+1));
		yield return new Routine(LogComputation.WaitSend(a+1,b));
		yield return new Routine(LogComputation.WaitSend(a+1,b+1));
	}
}
