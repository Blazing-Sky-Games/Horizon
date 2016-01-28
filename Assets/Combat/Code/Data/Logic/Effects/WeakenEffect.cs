using System;
using System.Collections;

public class WeakenEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("which stat will be changed")]
	public UnitStatatistic StatToWeaken = UnitStatatistic.Strength;

	[UnityEngine.Tooltip("Drop = M*potency")]
	public int DropM = 20;
	[UnityEngine.Tooltip("Duration = M*potency + B")]
	public int DurationB = 2;
	[UnityEngine.Tooltip("Duration = M*potency + B")]
	public int DurationM = 2;

	private int Potency;

	public override IEnumerator StartEffect ()
	{
		IUnitService unitService = ServiceLocator.GetService<IUnitService>();

		yield return new Routine(base.StartEffect());

		Potency = (int)GetMatchUp(unitService.GetUnit(Caster), unitService.GetUnit(Target), IsCritical);

		yield return new Routine(TurnsRemainingSetter.WaitSet(DurationM * Potency + DurationB));

		//drop
		throw new NotImplementedException();
	}

	public override IEnumerator EndEffect ()
	{
		//undo drop
		throw new NotImplementedException();
	}
}