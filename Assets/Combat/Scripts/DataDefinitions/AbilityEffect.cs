using System;
using System.Collections;

public class WaitForNextTurn{}

public abstract class AbilityEffect : UnityEngine.ScriptableObject
{
	public EffectType EffectType;

	public abstract IEnumerator WaitTrigger(Unit Attacker, Unit Defender,int abilityPower, bool IsCritical);

	// skill based multiplyer
	protected float GetCriticalPotency(Unit Attacker, Unit Defender)
	{
		float def = EffectType == EffectType.Physical ? Defender.Stability : Defender.Insight;

		return 1 + ((float)Attacker.Skill / (def * 2));
	}

	// combat strength based multiuplyer
	protected float GetCombatPotency(Unit Attacker, Unit Defender)
	{
		float atk = EffectType == EffectType.Physical ? Attacker.Strength : Attacker.Intelligence;
		float def = EffectType == EffectType.Physical ? Defender.Stability : Defender.Insight;

		return atk / def;
	}

	//TODO include ability power in this
	protected float GetPotency(Unit Attacker, Unit Defender, bool IsCritical)
	{
		return IsCritical ? GetCriticalPotency(Attacker, Defender) : GetCombatPotency(Attacker, Defender);
	}
}
