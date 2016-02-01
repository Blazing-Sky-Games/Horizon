using System;
using System.Collections;

class PoisonEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationB = 1;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationM = 3;

	private int dmg;

	public override IEnumerator WaitStart ()
	{
		yield return new Routine(base.WaitStart());

		float potency = GetMatchUp(Caster, Target, IsCritical);

		dmg = (int)(DamageM * potency);

		int duration = (int)(DurationB + DurationM * potency);

		yield return new Routine(DurationSetter.WaitSet(duration));
	}

	public override IEnumerator WaitUpdate ()
	{
		yield return new Routine(base.WaitUpdate());

		yield return new Routine(Target.Health.WaitHurt(dmg));
	}
}

