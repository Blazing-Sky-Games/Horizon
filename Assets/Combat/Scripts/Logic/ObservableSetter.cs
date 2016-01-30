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
