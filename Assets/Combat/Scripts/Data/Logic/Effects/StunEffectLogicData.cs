using System;
using System.Collections;

class StunEffectLogicData : AbilityEffectLogicData
{
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public float DurationM = 1.5f;
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public int DurationB = 1 ;

	public override IEnumerator WaitTrigger(UnitLogic Attacker, UnitLogic Defender, bool IsCritical)
	{
		//yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitStunEffect(Attacker, Defender, IsCritical))); TODO
		yield break;
	}

	IEnumerator WaitStunEffect(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, true));

		float Potency = GetMatchUp(attacker, defender, isCritical);
		int duration = DurationB + (int)((Potency - 1) * DurationM);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, false));
	}
}


