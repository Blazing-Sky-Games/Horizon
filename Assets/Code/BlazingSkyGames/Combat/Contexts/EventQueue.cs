/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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