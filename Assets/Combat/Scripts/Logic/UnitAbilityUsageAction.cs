using UnityEngine;
using System.Collections;

// represents and actors choice to use a unit ability
public class UnitAbilityUsageAction : IActorAction
{
	// the unit using the ability
	public UnitLogicData Caster;
	// the unit targeted by the ability
	public UnitLogicData Target;
	// the ability being used
	public UnitAbilityLogicData Ability;

	public UnitAbilityUsageAction(UnitLogicData caster, UnitAbilityLogicData ability, UnitLogicData target)
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
