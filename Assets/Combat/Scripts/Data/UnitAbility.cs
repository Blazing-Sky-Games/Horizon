using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

//TODO get rid of this, and creat messages with more generic arguments
public struct AbilityUsedMessageContent
{
	public readonly Unit Caster;
	public readonly Unit Target;
	public readonly UnitAbility Ability;

	public AbilityUsedMessageContent(Unit caster, UnitAbility ability, Unit target)
	{
		Caster = caster;
		Ability = ability;
		Target = target;
	}
}

public class UnitAbility : UnityEngine.ScriptableObject
{
	//suppled in editor
	public string AbilityName;
	public float CritChanceMultiplyer = 1;
	public List<AbilityEffect> CombatEffects;
	public List<AbilityEffect> CriticalEffects;

	public UnitAbility DeepCopy()
	{
		return UnityEngine.Object.Instantiate<UnitAbility>(this);
	}

	public IEnumerator WaitUse(Unit caster, Unit target)
	{
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(new AbilityUsedMessageContent(caster, this, target)));

		//later on, do somthing like this
		// startabilitymessage.send() to start animations
		// wait for abilityconnectedMessage or somthing like that,
		// then...

		foreach(AbilityEffect effect in CombatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		//TODO: should this be the formula for crit chance? also, it seems like skill is over powered because it makes all crits better
		//TODO: need formula that doen use effect type
//		float criticalSuccessThreshold = 
//				(EffectType == EffectType.Physical) ? (float)caster.Strength : (float)caster.Intelligence 
//			/ (float)target.Vitality 
//			/ 2 
//			* 0.4f; 

		//TODO: TALK ABOUT THIS MATH
		float criticalSuccessThreshold = 
				(float)caster.Strength 
				/ (float)target.Vitality 
				/ 2 
				* 0.4f; 

		if(Random.value < criticalSuccessThreshold * CritChanceMultiplyer)
		{
			//TODO send out critical hit message
			foreach(AbilityEffect effect in CriticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}

