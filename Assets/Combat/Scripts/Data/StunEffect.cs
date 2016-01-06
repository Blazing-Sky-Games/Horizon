using System;
using System.Collections;

class StunEffect : AbilityEffect
{
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public float DurationM = 1.5f;
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public int DurationB = 1 ;

	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, bool IsCritical)
	{
		yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitStunEffect(Attacker, Defender, IsCritical)));
	}

	IEnumerator WaitStunEffect(Unit attacker, Unit defender, bool isCritical)
	{
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, true));

		float Potency = GetPotency(attacker, defender, isCritical);
		int duration = DurationB + (int)((Potency - 1) * DurationM);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, false));
	}
}


