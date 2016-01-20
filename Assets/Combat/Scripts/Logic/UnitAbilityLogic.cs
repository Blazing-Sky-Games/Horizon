using System;
using System.Linq;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class UnitAbilityLogic : ViewLogic<UnitAbilityLogicData>
{
	private readonly List<AbilityEffectLogicData> combatEffects;
	private readonly List<AbilityEffectLogicData> criticalEffects;
	private readonly float critChanceBonus;

	public UnitAbilityLogic(UnitAbilityLogicData Data) 
		:base(Data)
	{
		combatEffects = data_copy.CombatEffects;
		criticalEffects = data_copy.CriticalEffects;
		critChanceBonus = data_copy.CritChanceBonus;
	}

	public IEnumerator WaitUse(UnitLogic caster, UnitLogic target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(AbilityEffectLogicData effect in combatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		//this equation will probably change
		float criticalSuccessThreshold = (caster.GetCriticalAccuracy() / target.GetCriticalAvoidance()) * 0.2f; 

		if(Random.value <= criticalSuccessThreshold + critChanceBonus)
		{
			//TODO send out critical hit message
			foreach(AbilityEffectLogicData effect in criticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}

	public override void Destroy ()
	{
		//nothing
	}
}


