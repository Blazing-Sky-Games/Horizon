using UnityEngine;
using System.Collections;

// represents an actors choice to use a unit ability
using Combat.Code.Services.TurnOrderService;


public class UnitAbilityUsageAction : IActorAction
{
	// the unit using the ability
	public UnitId Caster;
	// the unit targeted by the ability
	public UnitId Target;
	// the ability being used
	public UnitAbility Ability;

	public UnitAbilityUsageAction(UnitId caster, UnitAbility ability, UnitId target)
	{
		Caster = caster;
		Ability = ability;
		Target = target;
	}

	// execute this ability and wait for it to be finished
	public IEnumerator WaitPerform()
	{
		yield return new Routine(Ability.WaitUse(Caster, Target));
	}
}
