using System;
using System.Collections;

class PoisonEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationB = 1;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationM = 3;

	public override IEnumerator StartEffect ()
	{
		throw new NotImplementedException();
	}
	public override IEnumerator EndEffect ()
	{
		throw new NotImplementedException();
	}
}

