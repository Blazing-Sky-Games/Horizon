using System.Collections;

public class WeakenEffect : AbilityEffect
{
	public UnitStatatistic StatToWeaken = UnitStatatistic.Strength;

	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitWeakenEffect(Attacker, Defender, abilityPower, IsCritical));
	}

	IEnumerator WaitWeakenEffect(Unit attacker, Unit defender, int abilityPower, bool isCritical)
	{
		yield return defender.WaitSetStatus(UnitStatus.Weakened, true);

		float Potency = GetPotency(attacker, defender, isCritical);

		int drop = 20 * (int)Potency;
		int duration = 2 + (int)Potency * 2;

		int oldval = defender.GetStatistic(StatToWeaken);
		defender.SetStatistic(StatToWeaken, oldval - drop);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		defender.SetStatistic(StatToWeaken, oldval);
		yield return defender.WaitSetStatus(UnitStatus.Weakened, false);
	}
}



