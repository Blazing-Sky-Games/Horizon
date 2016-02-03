using System.Collections;

public class Observable<dataType>
{
	public readonly Message Changed = new Message();

	public dataType Value
	{
		get{ return m_value; }
	}

	public IEnumerator WaitSet(dataType value)
	{
		dataType old = m_value;
		m_value = value;

		if(!old.Equals(value))
			yield return new Routine(Changed.WaitSend());
	}

	public Observable(dataType value)
	{
		m_value = value;
	}

	public Observable()
	{
		m_value = default(dataType);
	}

	private dataType m_value;
}