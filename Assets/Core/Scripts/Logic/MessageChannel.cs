using System;
using System.Collections;
using System.Collections.Generic;

public class MessageChannel
{
	public IEnumerator Send ()
	{
		if (m_idle == false) {
			throw new InvalidOperationException ("send can only be called when a previose call to send has finised");
			//TODO should this be replaced with a qeue based system
			//what about recursive message calls (where fireing a message caused that message to fire again, like a recursive function call...would that ever happen)...hmmm
		}
		
		List<Coroutine> runningHandlers = new List<Coroutine> ();
		
		foreach (Func<IEnumerator> handler in m_handlers) {
			runningHandlers.Add (CoroutineManager.Main.StartCoroutine (handler ()));
		}
		
		m_idle = false;
		foreach (Coroutine runningHandler in runningHandlers) {
			yield return runningHandler;
		}
		
		// give a frame for calls to  "HandleMessage" to come in
		yield return 0;

		while (m_processors > 0) 
		{
			yield return 0;
		}
		
		m_idle = true;
	}
	
	public void AddHandler (Func<IEnumerator> handler)
	{
		m_handlers.Add (handler);
	}
	
	public void RemoveHandler (Func<IEnumerator> handler)
	{
		m_handlers.Remove(handler);
	}
	
	public IEnumerator HandleMessage (Func<IEnumerator> handler)
	{
		m_processors++;
		yield return handler ();
		m_processors--;
		
		while (MessagePending) 
		{
			yield return 0;
		}
	}
	
	public bool Idle {
		get {
			return m_idle;
		}
	}
	
	public bool MessagePending {
		get {
			return !Idle;
		}
	}
	
	private List<Func<IEnumerator>> m_handlers = new List<Func<IEnumerator>> ();
	private bool m_idle = true;
	private int m_processors;
}

public class MessageChannel<MessageContent>
{
	public IEnumerator Send (MessageContent content)
	{
		m_content = content;
		yield return m_innerMessage.Send ();
	}

	public void AddHandler (Func<MessageContent, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler)) 
			convertedHandlers[handler] = ConvertHandler (handler);

		m_innerMessage.AddHandler (convertedHandlers[handler]);
	}

	public void RemoveHandler (Func<MessageContent, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler)) 
			return;

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	public IEnumerator HandleMessage (Func<MessageContent, IEnumerator> handler)
	{
		yield return m_innerMessage.HandleMessage(ConvertHandler(handler));
	}
	
	public bool Idle {
		get {
			return m_innerMessage.Idle;
		}
	}
	
	public bool MessagePending {
		get {
			return m_innerMessage.MessagePending;
		}
	}
	
	private Func<IEnumerator> ConvertHandler (Func<MessageContent, IEnumerator> handler)
	{
		return () => handler (m_content);
	}

	private MessageChannel m_innerMessage = new MessageChannel();
	
	private MessageContent m_content;

	private static Dictionary<Func<MessageContent, IEnumerator>,Func<IEnumerator>> convertedHandlers = new Dictionary<Func<MessageContent, IEnumerator>, Func<IEnumerator>>();
}