using System;
using System.Collections;
using Combat.Code.Data.Logic.Effects;

class StunEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public float DurationM = 1.5f;
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public int DurationB = 1 ;


	public override IEnumerator StartEffect ()
	{
		throw new NotImplementedException();
	}
	public override IEnumerator EndEffect ()
	{
		throw new NotImplementedException();
	}
}


