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

using System;
using System.Collections;
using System.Collections.Generic;

public class Message : IGenericMessage
{
	private ICoroutineService coroutineService;

	public Message()
	{
		coroutineService = ServiceLocator.GetService<ICoroutineService>();
	}

	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend();
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public virtual IEnumerator WaitSend()
	{
		if(!m_idle)
		{
			throw new InvalidOperationException("send can only be called when a previose call to send has finised");
		}

		m_idle = false;
		
		List<Coroutine> runningHandlers = new List<Coroutine>();
		
		foreach(Func<IEnumerator> handler in m_handlers)
		{
			runningHandlers.Add(coroutineService.StartCoroutine(handler()));
		}

		foreach(Coroutine runningHandler in runningHandlers)
		{
			yield return runningHandler;
		}

		m_idle = true;
	}

	public void AddAction(Action handler)
	{
		if(!convertedActions.ContainsKey(handler))
		{ 
			convertedActions[handler] = convertAction(handler);
		}

		AddHandler(convertedActions[handler]);
	}

	public void RemoveAction(Action handler)
	{
		if(!convertedActions.ContainsKey(handler))
		{ 
			return;
		}

		RemoveHandler(convertedActions[handler]);
		convertedActions.Remove(handler);
	}

	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{ });
	}

	Func<IEnumerator> convertAction(Action handler)
	{
		return () => wrapAction(handler);
	}

	IEnumerator wrapAction(Action handler)
	{
		handler();
		yield break;
	}

	public void AddHandler(Func<IEnumerator> handler)
	{
		m_handlers.Add(handler);
	}
	
	public void RemoveHandler(Func<IEnumerator> handler)
	{
		m_handlers.Remove(handler);
	}

	private bool m_idle = true;
	private readonly List<Func<IEnumerator>> m_handlers = new List<Func<IEnumerator>>();
	private Dictionary<Action, Func<IEnumerator>> convertedActions = new Dictionary<Action, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}