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

 	int Potency;
	private int Drop;

	public override IEnumerator WaitStart ()
	{
		yield return new Routine(base.WaitStart());

		float Potency = GetMatchUp(Caster, Target, IsCritical);

		int duration = (int)(DurationM * Potency + DurationB);

		yield return new Routine(DurationSetter.WaitSet(duration));

		Drop = (int)(DropM * Potency);
		Target.GetStatistic(StatToWeaken).ApplyDrop(Drop);
	}

	public override IEnumerator WaitEnd ()
	{
		yield return new Routine(base.WaitEnd());
		Target.GetStatistic(StatToWeaken).RemoveDrop(Drop);
	}
}