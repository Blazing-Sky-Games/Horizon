using System;
using System.Collections;

class StunEffect : AbilityEffect
{
	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitStunEffect(Attacker, Defender, abilityPower, IsCritical));
	}

	IEnumerator WaitStunEffect(Unit attacker, Unit defender, int abilityPower, bool isCritical)
	{
		yield return defender.WaitSetStatus(UnitStatus.Stunned, true);

		float Potency = GetPotency(attacker, defender, isCritical);
		int duration = 1 + (int)Math.Floor((Potency - 1) * 1.5);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		yield return defender.WaitSetStatus(UnitStatus.Stunned, false);
	}
}


