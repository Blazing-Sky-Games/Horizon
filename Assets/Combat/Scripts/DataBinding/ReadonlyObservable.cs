using System.Collections;

public class ReadonlyObservable<dataType>
{
	public readonly Message Changed = new Message();

	public dataType Value
	{
		get{ return m_value; }
	}

	private IEnumerator WaitSet(dataType value)
	{
		dataType old = m_value;
		m_value = value;

		if(!old.Equals(value))
			yield return new Routine(Changed.WaitSend());
	}

	public ReadonlyObservable(dataType value, out ObservableSetter<dataType> setter)
	{
		m_value = value;
		setter = new ObservableSetter<dataType>(WaitSet);
	}

	public ReadonlyObservable(out ObservableSetter<dataType> setter)
	{
		m_value = default(dataType);
		setter = new ObservableSetter<dataType>(WaitSet);
	}

	private dataType m_value;
}
