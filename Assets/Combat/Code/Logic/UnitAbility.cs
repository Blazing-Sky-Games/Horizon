using System;
using System.Linq;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class UnitAbility
{
	private readonly List<CombatEffect> combatEffects;
	private readonly List<CombatEffect> criticalEffects;
	private readonly float critChanceBonus;

	public UnitAbility(UnitAbilityLogicData Data)
	{
		combatEffects = Data.CombatEffects;
		criticalEffects = Data.CriticalEffects;
		critChanceBonus = Data.CritChanceBonus;
	}

	public IEnumerator WaitUse(Unit caster, Unit target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(CombatEffect effect in combatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		//this equation will probably change
		float criticalSuccessThreshold = (caster.GetCriticalAccuracy() / target.GetCriticalAvoidance()) * 0.2f; 

		if(Random.value <= criticalSuccessThreshold + critChanceBonus)
		{
			//TODO send out critical hit message
			foreach(CombatEffect effect in criticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}


