using System.Collections;

public abstract class TurnBasedEffect : EnduringEffect
{
	public ReadonlyObservable<int> TurnsRemaining
	{
		get
		{
			return m_turnsRemaining;
		}
	}

	private ReadonlyObservable<int> m_turnsRemaining;
	protected ObservableSetter<int> TurnsRemainingSetter;

	public override IEnumerator StartEffect ()
	{
		m_turnsRemaining = new ReadonlyObservable<int>(out TurnsRemainingSetter);
		yield break;
	}

	public override IEnumerator OnNewTurn ()
	{
		int turnsRemainingMinusOne = TurnsRemaining.Value - 1;
		turnsRemainingMinusOne = turnsRemainingMinusOne < 0 ? 0 : turnsRemainingMinusOne;

		yield return new Routine(TurnsRemainingSetter.WaitSet(turnsRemainingMinusOne));
	}

	public override bool EndingCondition ()
	{
		return TurnsRemaining.Value == 0;
	}
}