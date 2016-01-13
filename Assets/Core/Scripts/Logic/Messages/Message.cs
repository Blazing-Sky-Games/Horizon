using System;
using System.Collections;
using System.Collections.Generic;

// A coroutine bassed event
// like a regular event, handlers can be added and removed, and the event can be "fired"
// but unlike an event, the handlers can be coroutines (take multiple frames) which run in parralel
// and "fireing" (sending) the message will not return untill all handlers have finished

// TODO do we need this next bit (late handlers)
// there is also a mechanism for seeing if a given message is "idle" or not (if send was just called)
// so code can handle a message being sent, without explicetly attaching a handler

public class Message
{
	//TODO return a coroutine and call it Send
	public IEnumerator WaitSend()
	{
		if(!Idle)
		{
			throw new InvalidOperationException("send can only be called when a previose call to send has finised");
			//TODO should this be replaced with a qeue based system
			//what about recursive message calls (where fireing a message caused that message to fire again, like a recursive function call...would that ever happen)...hmmm
		}
		
		List<Coroutine> runningHandlers = new List<Coroutine>();
		
		foreach(Func<IEnumerator> handler in m_handlers)
		{
			runningHandlers.Add(CoroutineManager.Main.StartCoroutine(handler()));
		}
		
		m_idle = false;
		foreach(Coroutine runningHandler in runningHandlers)
		{
			yield return runningHandler;
		}
		
		// give a frame for late handlers to show up
		// TODO hmm... should we even alow late handlers
		yield return 0;

		while(m_lateHandlers > 0)
		{
			yield return 0;
		}
		
		m_idle = true;
	}
	
	public void AddHandler(Func<IEnumerator> handler)
	{
		m_handlers.Add(handler);
	}
	
	public void RemoveHandler(Func<IEnumerator> handler)
	{
		m_handlers.Remove(handler);
	}
	
	//TODO return a coroutine and call it Handle
	public IEnumerator WaitHandleMessage(Func<IEnumerator> handler)
	{
		if(Idle)
		{
			throw new InvalidOperationException("Can not handle an idle message");
		}

		IEnumerator WaitHandler = handler();

		m_lateHandlers++;
		yield return new Routine(WaitHandler);
		m_lateHandlers--;
		
		while(MessagePending)
		{
			yield return 0;
		}
	}
	
	public bool Idle
	{
		get
		{
			return m_idle;
		}
	}
	
	public bool MessagePending
	{
		get
		{
			return !Idle;
		}
	}
	
	private bool m_idle = true;
	private int m_lateHandlers = 0;
	private readonly List<Func<IEnumerator>> m_handlers = new List<Func<IEnumerator>>();
}