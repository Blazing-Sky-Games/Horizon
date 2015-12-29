using UnityEngine;
using System.Collections;

// represents and actors choice to use a unit ability
public class UnitAbilityUsageAction : IActorAction
{
	// the unit using the ability
	public Unit caster;
	// the unit targeted by the ability
	public Unit target;
	// the ability being used
	public UnitAbility ability;

	public UnitAbilityUsageAction (Unit Caster, UnitAbility Ability, Unit Target)
	{
		caster = Caster;
		ability = Ability;
		target = Target;
	}

	// execute this ability and wait for it to be finished
	public IEnumerator WaitPerformAction ()
	{
		yield return ability.WaitStartUseAbility (caster, target);
	}
}
