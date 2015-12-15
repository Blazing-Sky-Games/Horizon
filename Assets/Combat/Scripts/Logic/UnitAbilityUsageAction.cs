using UnityEngine;
using System.Collections;

public class UnitAbilityUsageAction : IActorAction
{

	public Unit caster;
	public Unit target;
	public UnitAbility ability;

	public UnitAbilityUsageAction (Unit Caster, UnitAbility Ability, Unit Target)
	{
		caster = Caster;
		ability = Ability;
		target = Target;
	}

	public Coroutine WaitPerformAction ()
	{
		return ability.WaitStartUseAbility (caster, target);
	}
}
