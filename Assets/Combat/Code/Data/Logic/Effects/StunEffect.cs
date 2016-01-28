using System;
using System.Collections;

class StunEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public float DurationM = 1.5f;
	[UnityEngine.Tooltip("Duration = M*(Potency-1)+B")]
	public int DurationB = 1 ;

	public override IEnumerator WaitStart ()
	{
		Target.ActionPreventedPoll.AddVote();

		int duration = (int)(DurationM * GetMatchUp(Caster, Target, IsCritical) + DurationB);

		yield return new Routine(DurationSetter.WaitSet(duration));
	}
	public override IEnumerator WaitEnd ()
	{
		yield return new Routine(base.WaitEnd());
		Target.ActionPreventedPoll.RemoveVote();
	}
}