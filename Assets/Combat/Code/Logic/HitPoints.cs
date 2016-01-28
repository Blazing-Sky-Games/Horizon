using System.Collections;

public class HitPoints
{
	public readonly Message Hurt = new Message();
	public readonly Message MaxChanged = new Message();

	public readonly Message Zero = new Message();

	public int Current
	{
		get
		{
			return m_current;
		}
	}

	public int Max
	{
		get
		{
			return m_max;
		}
	}

	public HitPoints(int current, int maximum)
	{
		m_max = maximum;

		current = current < 0 ? 0 : current;
		current = current > maximum ? maximum : current;

		m_current = current;
	}

	public IEnumerator WaitHurt(int dmg)
	{
		m_current -= dmg;
		m_current = m_current < 0 ? 0 : Current;

		yield return new Routine(Hurt.WaitSend());

		if(m_current == 0)
		{
			yield return new Routine(Zero.WaitSend());
		}
	}

	public IEnumerator WaitSetMax(int max)
	{
		m_max = max;

		yield return new Routine(MaxChanged.WaitSend());

		if(m_current > m_max)
		{
			yield return new Routine(WaitHurt(m_current - m_max));
		}
	}

	private int m_current;
	private int m_max;
}