using System.Collections.Generic;

public interface GenericEventQueue
{
	void DynamicSend (object[] args);
}

public class EventQueue<T> : GenericEventQueue
{
	#region GenericEventQueue implementation

	public void DynamicSend (object[] args)
	{
		Send((T)args[0]);
	}

	#endregion

	public bool EventPending
	{
		get
		{
			return events.Count > 0;
		}
	}

	public void Clear()
	{
		events.Clear();
	}

	public T ConsumeEvent()
	{
		return events.Dequeue();
	}

	public void Send(T Event)
	{
		events.Enqueue(Event);
	}

	private Queue<T> events = new Queue<T>();
}

public class EventQueue : GenericEventQueue
{
	#region GenericEventQueue implementation

	public void DynamicSend (object[] args)
	{
		Send();
	}

	#endregion

	public bool EventPending
	{
		get
		{
			return events > 0;
		}
	}

	public void Clear()
	{
		events = 0;
	}

	public void ConsumeEvent()
	{
		events--;
	}

	public void Send()
	{
		events++;
	}

	private int events;
}