using System.Collections;
using System;

[InlineData]
public abstract class AbilityEffectLogicData : Data
{
	public EffectType effectType;

	public abstract IEnumerator WaitTrigger(UnitLogic attacker, UnitLogic defender, bool isCritical);

	// relative comparason between units for a critical effect
	private float GetCriticalMatchUp(UnitLogic attacker, UnitLogic defender)
	{
		return 1 + ((float)attacker.GetCriticalPotency(effectType) / (defender.GetCriticalResistance(effectType) * 2));
	}

	// relative comparason between units for a combat effect
	private float GetCombatMatchUp(UnitLogic attacker, UnitLogic defender)
	{
		return attacker.GetCombatPotency(effectType) / defender.GetCombatResistance(effectType);
	}

	// relative comparason between units for an combat
	protected float GetMatchUp(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		return isCritical ? GetCriticalMatchUp(attacker, defender) : GetCombatMatchUp(attacker, defender);
	}
}
