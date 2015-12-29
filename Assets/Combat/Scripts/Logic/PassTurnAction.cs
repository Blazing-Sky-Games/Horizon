using UnityEngine;
using System.Collections;

public class PassTurnAction : IActorAction
{
	public IEnumerator WaitPerformAction ()
	{
		yield return null;
	}
}
