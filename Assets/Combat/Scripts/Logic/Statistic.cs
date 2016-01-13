using System;
using System.Collections;

public class Statistic<DataType>
{
	public readonly Message Changed = new Message();

	public DataType Get()
	{
		return m_value;
	}

	public IEnumerator WaitSet(DataType value)
	{
		DataType old = m_value;
		m_value = value;

		if(!old.Equals(value))
			yield return new Routine(Changed.WaitSend());
	}

	public Statistic(DataType value)
	{
		m_value = value;
	}

	private DataType m_value;
}


