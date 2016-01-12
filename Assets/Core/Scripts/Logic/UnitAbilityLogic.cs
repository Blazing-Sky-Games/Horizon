using System;
using System.Collections;
using Random = UnityEngine.Random;

public class UnitAbilityLogic
{
	public UnitAbilityLogicData data;

	public UnitAbilityLogic(UnitAbilityLogicData data)
	{
		this.data = data;
	}

	public IEnumerator WaitUse(UnitLogic caster, UnitLogic target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		//later on, do somthing like this
		// startabilitymessage.send() to start animations
		// wait for abilityconnectedMessage or somthing like that,
		// then...

		foreach(AbilityEffectLogicData effect in data.CombatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		//TODO: TALK ABOUT THIS MATH
		float criticalSuccessThreshold = 
			(float)caster.data.Strength 
			/ (float)target.data.Vitality 
			/ 2 
			* 0.4f; 

		if(Random.value < criticalSuccessThreshold * data.CritChanceMultiplyer)
		{
			//TODO send out critical hit message
			foreach(AbilityEffectLogicData effect in data.CriticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}


