﻿using System;
using System.Collections;
using System.Collections.Generic;

//CODE GENERATED BY T4
//DO NOT EDIT BY HAND
//EDIT THE .tt FILE, and use monodevlop to regenerate
//see http://dan.s.wards.ws/how-to-execute-t4-text-templates-in-monodevelop/

public class Message<TArg0>
{
	public delegate IEnumerator Handler(TArg0 arg0);
	
	public IEnumerator WaitSend(TArg0 arg0)
	{
        m_arg0 = arg0;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0);
	}

    private TArg0 m_arg0;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1>
{
	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1);
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2>
{
	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2);
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2, TArg3>
{
	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
        m_arg3 = arg3;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2, m_arg3);
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
    private TArg3 m_arg3;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2, TArg3, TArg4>
{
	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
        m_arg3 = arg3;
        m_arg4 = arg4;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			convertedHandlers[handler] = ConvertHandler(handler);
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
	}

	public void RemoveHandler(Handler handler)
	{
		if(!convertedHandlers.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.AddHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
    private TArg3 m_arg3;
    private TArg4 m_arg4;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
}
