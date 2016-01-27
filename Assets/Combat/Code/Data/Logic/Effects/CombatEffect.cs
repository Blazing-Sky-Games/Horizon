using System.Collections;
using System;
using Combat.Code.Services.TurnOrderService;

[InlineData]
public abstract class CombatEffect : Data
{
	public EffectType effectType;

	public abstract IEnumerator WaitTrigger(UnitId attacker, UnitId defender, bool isCritical);

	// relative comparason between units for a critical effect
	private float GetCriticalMatchUp(Unit attacker, Unit defender)
	{
		return 1 + ((float)attacker.GetCriticalPotency(effectType) / (defender.GetCriticalResistance(effectType) * 2));
	}

	// relative comparason between units for a combat effect
	private float GetCombatMatchUp(Unit attacker, Unit defender)
	{
		return attacker.GetCombatPotency(effectType) / defender.GetCombatResistance(effectType);
	}

	// relative comparason between units for an combat
	protected float GetMatchUp(Unit attacker, Unit defender, bool isCritical)
	{
		return isCritical ? GetCriticalMatchUp(attacker, defender) : GetCombatMatchUp(attacker, defender);
	}
}
