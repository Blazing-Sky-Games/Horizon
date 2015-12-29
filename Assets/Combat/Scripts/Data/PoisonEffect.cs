using System;
using System.Collections;

//TODO and this goes for other turn based effects. nead an elegant way to handle a unit dieing
class PoisonEffect : AbilityEffect
{
	// tuning variable
	private const float DURCOEF = 3;
	// the minimum number of turns posioning lasts
	private const int MINDUR = 3;

	public override IEnumerator WaitTrigger(Unit attacker, Unit defender, int abilityPower, bool isCritical)
	{
		yield return TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitPoisonEffect(attacker, defender, isCritical));
	}

	private IEnumerator WaitPoisonEffect(Unit attacker, Unit defender, bool isCritical)
	{
		yield return defender.WaitSetStatus(UnitStatus.Poisoned, true);

		float potency = GetPotency(attacker, defender, isCritical);

		//how much damage does the posion do
		int dmg = (int)(10 * potency);

		//how many turns does the poison last
		int duration = (int)(MINDUR + DURCOEF * potency);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
			yield return defender.WaitTakeDamage((int)dmg, false);
		}

		yield return defender.WaitSetStatus(UnitStatus.Poisoned, false);
	}
}

