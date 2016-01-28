// represents an actors choice to use a unit ability
using System.Collections;


public class UnitAbilityUsageAction : IActorAction
{
	// the unit using the ability
	public Unit Caster;
	// the unit targeted by the ability
	public Unit Target;
	// the ability being used
	public UnitAbility Ability;

	public UnitAbilityUsageAction(Unit caster, UnitAbility ability, Unit target)
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