using System;
using System.Collections;

class StunEffect : AbilityEffect
{
	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, bool IsCritical)
	{
		yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitStunEffect(Attacker, Defender, IsCritical)));
	}

	IEnumerator WaitStunEffect(Unit attacker, Unit defender, bool isCritical)
	{
		//TODO impliment stun
		throw new NotImplementedException("Implement stun stupid!");

		//yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, true));

		//float Potency = GetPotency(attacker, defender, isCritical);
		//int duration = 1 + (int)Math.Floor((Potency - 1) * 1.5);

		//while(duration > 0)
		//{
		//yield return new WaitForNextTurn();
		//duration--;
		//}

		//yield return new Routine(defender.WaitSetStatus(UnitStatus.Stunned, false));
	}
}


