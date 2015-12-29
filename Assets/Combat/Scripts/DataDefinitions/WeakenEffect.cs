using System;
using System.Collections;

public enum UnitStat
{
	Strength, // phys dmg /crit
	Intelligence, // tech dmg /crit
	Stability, // phys def/crit res
	Insight, // tech def/crit res
	Skill, // crit dmg
	Vitality // - crit chance for defender. also HP
}

public class WeakenEffect : AbilityEffect
{
	public UnitStat StatToWeaken = UnitStat.Strength;

	public override IEnumerator Trigger (Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return TurnBasedEffectManager.StartTurnBasedEffect(WeakenTurnBasedEffect(Attacker,Defender,abilityPower,IsCritical));
	}

	IEnumerator WeakenTurnBasedEffect(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		yield return Defender.SetStatus(UnitStatus.Weakened, true);

		float Potency = GetPotency(Attacker,Defender,IsCritical);

		int drop = 20 * (int)Potency;
		int duration = 2 + (int)Potency * 2;

		int oldval = Defender.GetStat(StatToWeaken);
		Defender.SetStat(StatToWeaken,oldval - drop);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		Defender.SetStat(StatToWeaken,oldval);
		yield return Defender.SetStatus(UnitStatus.Weakened, false);
	}
}



