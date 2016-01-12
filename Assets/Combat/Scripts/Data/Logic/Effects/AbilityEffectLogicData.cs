using System.Collections;
using System;

[InlineData]
public abstract class AbilityEffectLogicData : Data
{
	public EffectType effectType;

	public abstract IEnumerator WaitTrigger(UnitLogic attacker, UnitLogic defender, bool isCritical);

	// skill based multiplyer
	protected float GetCriticalPotency(UnitLogicData attacker, UnitLogicData defender)
	{
		float def = effectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return 1 + ((float)attacker.Skill / (def * 2));
	}

	// combat strength based multiuplyer
	protected float GetCombatPotency(UnitLogicData attacker, UnitLogicData defender)
	{
		float atk = effectType == EffectType.Physical ? attacker.Strength : attacker.Intelligence;
		float def = effectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return atk / def;
	}

	//TODO include ability power in this
	protected float GetPotency(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		return isCritical ? GetCriticalPotency(attacker.data, defender.data) : GetCombatPotency(attacker.data, defender.data);
	}
}
