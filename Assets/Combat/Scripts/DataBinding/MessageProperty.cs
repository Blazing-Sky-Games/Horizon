using System.Collections;

public class MessageProperty
{
	public readonly Message<object> Changing;
	public readonly Message<object> Changed;

	public object ObjectValue
	{
		get{ return m_value; }
	}

	public IEnumerator WaitSet(object value)
	{
		object old = m_value;

		if(old != null && value != null && old.GetType() != value.GetType())
		{
			throw new System.InvalidOperationException("cannot change type of data in message property. set to null to change type");
		}

		if((old == null && m_value == null) || !old.Equals(value))
			yield return new Routine(Changing.WaitSend(value));

		m_value = value;

		if((old == null && m_value == null) || !old.Equals(value))
			yield return new Routine(Changed.WaitSend(m_value));
	}

	public MessageProperty(object value)
	{
		m_value = value;
	}

	public MessageProperty()
	{
		m_value = default(object);
	}

	private object m_value;
}

public class MessageProperty<dataType> : MessageProperty
{
	public dataType Value
	{
		get{ return (dataType)ObjectValue; }
	}

	public IEnumerator WaitSet(dataType value)
	{
		yield return new Routine(WaitSet((object)value));
	}

	public MessageProperty(dataType value) : base(value)
	{
	}

	public MessageProperty() : base(default(dataType))
	{
	}
}