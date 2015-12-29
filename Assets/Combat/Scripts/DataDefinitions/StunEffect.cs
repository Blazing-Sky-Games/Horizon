using System;
using System.Collections;

class StunEffect : AbilityEffect
{
	public override IEnumerator WaitTrigger (Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitStunEffect(Attacker,Defender,abilityPower,IsCritical));
	}

	IEnumerator WaitStunEffect(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return Defender.WaitSetStatus(UnitStatus.Stunned, true);

		float Potency = GetPotency(Attacker,Defender,IsCritical);
		int duration = 1 + (int)Math.Floor((Potency - 1) * 1.5);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		yield return Defender.WaitSetStatus(UnitStatus.Stunned, false);
	}
}


