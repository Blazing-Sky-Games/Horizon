using System;
using System.Collections;

class StunEffect : AbilityEffect
{
	public override IEnumerator Trigger (Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.StartTurnBasedEffect(StunTurnBasedEffect(Attacker,Defender,abilityPower,IsCritical));
	}

	IEnumerator StunTurnBasedEffect(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return Defender.SetStatus(UnitStatus.Stunned, true);

		float Potency = GetPotency(Attacker,Defender,IsCritical);
		int duration = 1 + (int)Math.Floor((Potency - 1) * 1.5);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		yield return Defender.SetStatus(UnitStatus.Stunned, false);
	}
}


