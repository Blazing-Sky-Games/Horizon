using UnityEngine;
using System.Collections;

public class PassTurnAction : IActorAction
{
	public IEnumerator WaitPerform ()
	{
		yield return null;
	}
}
