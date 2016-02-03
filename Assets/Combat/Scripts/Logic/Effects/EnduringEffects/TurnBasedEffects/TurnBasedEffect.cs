using System.Collections;

public abstract class TurnBasedEffect : EnduringEffect
{
	public ReadonlyObservable<int> Duration
	{
		get
		{
			return m_duration;
		}
	}

	private ReadonlyObservable<int> m_duration;
	protected ObservableSetter<int> DurationSetter;

	public override IEnumerator WaitStart ()
	{
		m_duration = new ReadonlyObservable<int>(out DurationSetter);
		yield break;
	}

	public override IEnumerator WaitUpdate ()
	{
		int turnsRemainingMinusOne = Duration.Value - 1;
		turnsRemainingMinusOne = turnsRemainingMinusOne < 0 ? 0 : turnsRemainingMinusOne;

		yield return new Routine(DurationSetter.WaitSet(turnsRemainingMinusOne));
	}

	public override bool EndingCondition ()
	{
		return Duration.Value == 0;
	}
}