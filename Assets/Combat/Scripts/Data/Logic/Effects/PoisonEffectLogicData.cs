using System;
using System.Collections;

//TODO and this goes for other turn based effects. nead an elegant way to handle a unit dieing
class PoisonEffectLogicData : AbilityEffectLogicData
{
	[UnityEngine.Tooltip("dmg = M*Potency")]
	public int DamageM = 10;

	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationB = 1;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationM = 3;

	public override IEnumerator WaitTrigger(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitPoisonEffect(attacker, defender, isCritical)));
	}

	private IEnumerator WaitPoisonEffect(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Poisoned, true));

		float potency = GetMatchUp(attacker, defender, isCritical);

		//how much damage does the posion do
		int dmg = (int)(DamageM * potency);

		//how many turns does the poison last
		int duration = (int)(DurationB + DurationM * potency);

		//while(duration > 0 && !defender.Dead)
		//{
			yield return new WaitForNextTurn();
			duration--;

			//TODO handle this better
			//if(!defender.Dead)
			//{
				yield return new Routine(defender.Health.WaitHurt((int)dmg));
			//}
		//}

		yield return new Routine(defender.WaitSetStatus(UnitStatus.Poisoned, false));
	}
}

