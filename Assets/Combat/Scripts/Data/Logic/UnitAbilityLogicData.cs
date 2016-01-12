using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DataCatagory("Combat/Logic")]
public class UnitAbilityLogicData : Data
{
	//suppled in editor
	public string AbilityName;
	public float CritChanceMultiplyer = 1;
	public List<AbilityEffectLogicData> CombatEffects;
	public List<AbilityEffectLogicData> CriticalEffects;

	public IEnumerator WaitUse(UnitLogicData caster, UnitLogicData target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		//later on, do somthing like this
		// startabilitymessage.send() to start animations
		// wait for abilityconnectedMessage or somthing like that,
		// then...

		foreach(AbilityEffectLogicData effect in CombatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		//TODO: TALK ABOUT THIS MATH
		float criticalSuccessThreshold = 
				(float)caster.Strength 
				/ (float)target.Vitality 
				/ 2 
				* 0.4f; 

		if(Random.value < criticalSuccessThreshold * CritChanceMultiplyer)
		{
			//TODO send out critical hit message
			foreach(AbilityEffectLogicData effect in CriticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}

