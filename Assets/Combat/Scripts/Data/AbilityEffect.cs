using System.Collections;

public abstract class AbilityEffect : UnityEngine.ScriptableObject
{
	public EffectType EffectType;

	public abstract IEnumerator WaitTrigger(Unit attacker, Unit defender, int abilityPower, bool isCritical);

	// skill based multiplyer
	protected float GetCriticalPotency(Unit attacker, Unit defender)
	{
		float def = EffectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return 1 + ((float)attacker.Skill / (def * 2));
	}

	// combat strength based multiuplyer
	protected float GetCombatPotency(Unit attacker, Unit defender)
	{
		float atk = EffectType == EffectType.Physical ? attacker.Strength : attacker.Intelligence;
		float def = EffectType == EffectType.Physical ? defender.Stability : defender.Insight;

		return atk / def;
	}

	//TODO include ability power in this
	protected float GetPotency(Unit attacker, Unit defender, bool isCritical)
	{
		return isCritical ? GetCriticalPotency(attacker, defender) : GetCombatPotency(attacker, defender);
	}
}
