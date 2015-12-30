using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class StackHelper
{
	
	public static String ReportError(string Message)
	{
		// Get the frame one step up the call tree
		StackFrame CallStack = new StackFrame(1, true);
		
		// These will now show the file and line number of the ReportError
		string SourceFile = CallStack.GetFileName();
		int SourceLine = CallStack.GetFileLineNumber();
		
		return "Error: " + Message + "\nFile: " + SourceFile + "\nLine: " + SourceLine.ToString();
	}
	
	public static int Caller__LINE__
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			int line = new int();
			line += CallStack.GetFileLineNumber();
			return line;
		}
	}
	
	public static string Caller__FILE__
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			string temp = CallStack.GetFileName();
			String file = String.Copy(String.IsNullOrEmpty(temp) ? "" : temp);
			return String.IsNullOrEmpty(file) ? "" : file;
		}
	}

	public static string Caller__METHOD__
	{
		get
		{
			StackFrame CallStack = new StackFrame(2, true);
			string temp = CallStack.GetMethod().Name;
			String meth = String.Copy(String.IsNullOrEmpty(temp) ? "" : temp);
			return String.IsNullOrEmpty(meth) ? "" : meth;
		}
	}
}

public class Message
{
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

public class Message<TArg>
{
	public IEnumerator WaitSend(TArg arg)
	{
		m_arg = arg;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Func<TArg, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Func<TArg, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	public IEnumerator WaitHandleMessage(Func<TArg, IEnumerator> handler)
	{
		yield return new Routine(m_innerMessage.WaitHandleMessage(ConvertHandler(handler)));
	}
	
	public bool Idle
	{
		get
		{
			return m_innerMessage.Idle;
		}
	}
	
	public bool MessagePending
	{
		get
		{
			return m_innerMessage.MessagePending;
		}
	}
	
	private Func<IEnumerator> ConvertHandler(Func<TArg, IEnumerator> handler)
	{
		return () => handler(m_arg);
	}

	private TArg m_arg;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Func<TArg, IEnumerator>,Func<IEnumerator>> convertedHandlers = new Dictionary<Func<TArg, IEnumerator>, Func<IEnumerator>>();
}

public class Message<TArg0,TArg1>
{
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1)
	{
		m_arg0 = arg0;
		m_arg1 = arg1;
		yield return new Routine(m_innerMessage.WaitSend());
	}
	
	public void AddHandler(Func<TArg0, TArg1, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}
		
		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}
	
	public void RemoveHandler(Func<TArg0, TArg1, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}
		
		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	public IEnumerator WaitHandleMessage(Func<TArg0, TArg1, IEnumerator> handler)
	{
		yield return new Routine(m_innerMessage.WaitHandleMessage(ConvertHandler(handler)));
	}
	
	public bool Idle
	{
		get
		{
			return m_innerMessage.Idle;
		}
	}
	
	public bool MessagePending
	{
		get
		{
			return m_innerMessage.MessagePending;
		}
	}
	
	private Func<IEnumerator> ConvertHandler(Func<TArg0, TArg1, IEnumerator> handler)
	{
		return () => handler(m_arg0, m_arg1);
	}
	
	private TArg0 m_arg0;
	private TArg1 m_arg1;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Func<TArg0, TArg1, IEnumerator>,Func<IEnumerator>> convertedHandlers = new Dictionary<Func<TArg0, TArg1, IEnumerator>, Func<IEnumerator>>();
}

public class Message<TArg0,TArg1,TArg2>
{
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2)
	{
		m_arg0 = arg0;
		m_arg1 = arg1;
		m_arg2 = arg2;
		yield return new Routine(m_innerMessage.WaitSend());
	}
	
	public void AddHandler(Func<TArg0, TArg1,TArg2, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}
		
		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}
	
	public void RemoveHandler(Func<TArg0, TArg1,TArg2, IEnumerator> handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}
		
		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	public IEnumerator WaitHandleMessage(Func<TArg0, TArg1,TArg2, IEnumerator> handler)
	{
		yield return new Routine(m_innerMessage.WaitHandleMessage(ConvertHandler(handler)));
	}
	
	public bool Idle
	{
		get
		{
			return m_innerMessage.Idle;
		}
	}
	
	public bool MessagePending
	{
		get
		{
			return m_innerMessage.MessagePending;
		}
	}
	
	private Func<IEnumerator> ConvertHandler(Func<TArg0, TArg1,TArg2, IEnumerator> handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2);
	}
	
	private TArg0 m_arg0;
	private TArg1 m_arg1;
	private TArg2 m_arg2;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Func<TArg0, TArg1,TArg2, IEnumerator>,Func<IEnumerator>> convertedHandlers = new Dictionary<Func<TArg0, TArg1,TArg2, IEnumerator>, Func<IEnumerator>>();
}