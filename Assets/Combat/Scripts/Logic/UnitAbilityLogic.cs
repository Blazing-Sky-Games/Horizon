using System;
using System.Collections;
using Random = UnityEngine.Random;

public class UnitAbilityLogic : DataDrivenLogic<UnitAbilityLogicData>
{
	public UnitAbilityLogic(UnitAbilityLogicData Data) 
		:base(Data)
	{
	}

	public IEnumerator WaitUse(UnitLogic caster, UnitLogic target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(AbilityEffectLogicData effect in Data.CombatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		float criticalSuccessThreshold = (caster.GetCriticalChance() + Data.CritChanceBonus) * (1 - target.GetCriticalAvoidanceChance()); 

		if(Random.value <= criticalSuccessThreshold)
		{
			//TODO send out critical hit message
			foreach(AbilityEffectLogicData effect in Data.CriticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}


