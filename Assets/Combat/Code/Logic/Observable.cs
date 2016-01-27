using System;
using System.Collections;

public class ObservableSetter<dataType>
{
	private readonly Func<dataType,IEnumerator> m_setFunction;

	public ObservableSetter(Func<dataType,IEnumerator> setFunction)
	{
		m_setFunction = setFunction;
	}

	public IEnumerator WaitSet(dataType value)
	{
		yield return new Routine(m_setFunction(value));
	}
}

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


