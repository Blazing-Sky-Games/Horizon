using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Contexts;

//generic interface to all types of messages
public interface IGenericMessage
{
	IEnumerator WaitSendGeneric (object[] args);
	void AddHandlerGeneric(Func<object[],IEnumerator> handler);
	void RemoveHandlerGeneric(Func<object[],IEnumerator> handler);
}

// A coroutine based event
// like a regular event, handlers can be added and removed, and the event can be "fired"
// but unlike an event, the handlers can be coroutines (take multiple frames) which run in parralel
// and "fireing" (sending) the message will not return untill all handlers have finished
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

	public IEnumerator WaitSend()
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