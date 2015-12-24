using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class UnitAbility : ScriptableObject
{
	//suppled in editor
	public int power;
	public DmgType damageType;
	public string conditioncause;
	public string AbilityName;

	public UnitAbility DeepCopy()
	{
		return UnityEngine.Object.Instantiate<UnitAbility> (this);
	}

	public Coroutine WaitStartUseAbility (Unit Caster, Unit Target)
	{
		bool crit = false;
		int dmg = Caster.CalcDamageAgainst (power, damageType, Target, conditioncause, out crit);

		//notify that this ability has been used (play animations, sounds, trigger hurt animations, trigger dmg, trigger death.... etc)
		Caster.AbilityUsedMessage.SendMessage (new AbilityUsedMessageContent(Caster,this,Target,dmg,crit));
		return Caster.AbilityUsedMessage.WaitTillMessageProcessed ();
	}

	public IEnumerator AffectTarget(AbilityUsedMessageContent content)
	{
		return content.Target.RespondToAttackRoutine(content.Dmg);
	}
}

