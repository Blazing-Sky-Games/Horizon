using System;
using System.Collections;

//TODO and this goes for other turn based effects. nead an elegant way to handle a unit dieing
class PoisonEffect : AbilityEffect
{
	// tuning variable
	private const float DURCOEF = 3;
	// the minimum number of turns posioning lasts
	private const int MINDUR = 3;

	public override IEnumerator Trigger (Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.StartTurnBasedEffect(PoisonRoutine(Attacker,Defender,IsCritical));
	}

	private IEnumerator PoisonRoutine(Unit Attacker, Unit Defender, bool IsCritical)
	{
		yield return Defender.SetStatus(UnitStatus.Poisoned, true);

		float potency = GetPotency(Attacker, Defender, IsCritical);

		//how much damage does the posion do
		int dmg = (int) ( 10 * potency );

		//how many turns does the poison last
		int duration = (int) ( MINDUR + DURCOEF * potency );

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
			yield return Defender.TakeDamage((int)dmg,false);
		}

		yield return Defender.SetStatus(UnitStatus.Poisoned, false);
	}
}

