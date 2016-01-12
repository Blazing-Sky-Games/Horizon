using System.Collections;
using System;

[InlineData]
public abstract class AbilityEffect : Data
{
	public EffectType effectType;

	public abstract IEnumerator WaitTrigger(Unit attacker, Unit defender, bool isCritical);

	// skill based multiplyer
	protected float GetCriticalPotency(Unit attacker, Unit defender)
	{
		float def = effectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return 1 + ((float)attacker.Skill / (def * 2));
	}

	// combat strength based multiuplyer
	protected float GetCombatPotency(Unit attacker, Unit defender)
	{
		float atk = effectType == EffectType.Physical ? attacker.Strength : attacker.Intelligence;
		float def = effectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return atk / def;
	}

	//TODO include ability power in this
	protected float GetPotency(Unit attacker, Unit defender, bool isCritical)
	{
		return isCritical ? GetCriticalPotency(attacker, defender) : GetCombatPotency(attacker, defender);
	}
}
