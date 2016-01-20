using System;
using System.Collections;

public class Statistic
{
	public readonly Message Changed = new Message();

	public int Value
	{
		get{ return m_value + m_modifications; }
	}

	//TODO creat a "modification" class, to keep better track of the idividual effects changing a stat?
	public IEnumerator WaitModify(int value)
	{
		int old = m_modifications;
		m_modifications = value;

		if(!old.Equals(value))
			yield return new Routine(Changed.WaitSend());
	}

	public IEnumerator WaitSet(int value)
	{
		int old = m_value;
		m_value = value;

		if(!old.Equals(value))
			yield return new Routine(Changed.WaitSend());
	}

	public Statistic(int value)
	{
		m_value = value;
	}

	private int m_value;
	private int m_modifications;
}


