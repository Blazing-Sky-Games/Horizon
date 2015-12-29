using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public struct AbilityUsedMessageContent
{
	public readonly Unit Caster;
	public readonly Unit Target;
	public readonly int Dmg;
	public readonly bool Crit;
	public readonly UnitAbility Ability;

	public AbilityUsedMessageContent(Unit Caster, UnitAbility Ability,Unit Target, int Dmg, bool Crit)
	{
		this.Caster = Caster;
		this.Ability = Ability;

		this.Target = Target;
		
		this.Dmg = Dmg;
		this.Crit = Crit;
	}
}

public class UnitAbility : UnityEngine.ScriptableObject
{
	//suppled in editor
	public EffectType effectType;
	public string AbilityName;

	public int AbilityPower = 10;
	public float CritChanceMultiplyer = 1;

	public List<AbilityEffect> CombatEffects;
	public List<AbilityEffect> CriticalEffects;

	public UnitAbility DeepCopy()
	{
		return UnityEngine.Object.Instantiate<UnitAbility> (this);
	}

	public IEnumerator WaitUse (Unit Caster, Unit Target)
	{
		yield return Caster.AbilityUsedMessage.WaitSend(new AbilityUsedMessageContent(Caster,this,Target,0,false));

		//later on, do somthing like this
		// startabilitymessage.send() to start animations
		// wait for abilityconnectedMessage or somthing like that,
		// then...

		foreach(AbilityEffect effect in CombatEffects)
		{
			yield return effect.WaitTrigger(Caster,Target,AbilityPower,false);
		}

		//TODO: should this be the formula for crit chance? also, it seems like skill is over powered because it makes all crits better
		float criticalSuccessThreshold = 
				(effectType == EffectType.Physical) ? (float)Caster.Strength : (float)Caster.Intelligence 
				/ (float)Target.Vitality 
				/ 2 
				* 0.4f; 

		if(Random.value < criticalSuccessThreshold * CritChanceMultiplyer)
		{
			//TODO send out critical hit message
			foreach(AbilityEffect effect in CriticalEffects)
			{
				yield return effect.WaitTrigger(Caster,Target,AbilityPower,true);
			}
		}
	}
}

