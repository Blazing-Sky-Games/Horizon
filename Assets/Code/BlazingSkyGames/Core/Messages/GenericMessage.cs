﻿/*
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

//CODE GENERATED BY T4
//DO NOT EDIT BY HAND
//EDIT THE .tt FILE, and use xamarin studio to regenerate
// add the tt file back to the project (unit removes it automatically)
//set custom tool to TextTemplatingFileGenerator see https://forums.xamarin.com/discussion/7573/run-t4-template-in-xamarin-studio

public class Message<TArg0> : IGenericMessage
{
	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend((TArg0)args[0]);
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		m_innerMessage.AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		m_innerMessage.RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public delegate IEnumerator Handler(TArg0 arg0);
	public delegate void HandlerAction(TArg0 arg0);
	
	public IEnumerator WaitSend(TArg0 arg0)
	{
        m_arg0 = arg0;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			convertedHandlerActions[handler] = ConvertHandlerAction(handler);
		}

		m_innerMessage.AddHandler(convertedHandlerActions[handler]);
	}

	public void RemoveAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.RemoveHandler(convertedHandlerActions[handler]);
		convertedHandlerActions.Remove(handler);
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

		m_innerMessage.RemoveHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{m_arg0 });
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0);
	}

	private Func<IEnumerator> ConvertHandlerAction(HandlerAction handler)
	{
		return () => WrapAction(handler);
	}

	private IEnumerator WrapAction(HandlerAction handler)
	{
		handler(m_arg0);
		yield break;
	}

    private TArg0 m_arg0;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
	private static readonly Dictionary<HandlerAction,Func<IEnumerator>> convertedHandlerActions = new Dictionary<HandlerAction, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1> : IGenericMessage
{
	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend((TArg0)args[0], (TArg1)args[1]);
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		m_innerMessage.AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		m_innerMessage.RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1);
	public delegate void HandlerAction(TArg0 arg0, TArg1 arg1);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			convertedHandlerActions[handler] = ConvertHandlerAction(handler);
		}

		m_innerMessage.AddHandler(convertedHandlerActions[handler]);
	}

	public void RemoveAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.RemoveHandler(convertedHandlerActions[handler]);
		convertedHandlerActions.Remove(handler);
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

		m_innerMessage.RemoveHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{m_arg0, m_arg1 });
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1);
	}

	private Func<IEnumerator> ConvertHandlerAction(HandlerAction handler)
	{
		return () => WrapAction(handler);
	}

	private IEnumerator WrapAction(HandlerAction handler)
	{
		handler(m_arg0, m_arg1);
		yield break;
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
	private static readonly Dictionary<HandlerAction,Func<IEnumerator>> convertedHandlerActions = new Dictionary<HandlerAction, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2> : IGenericMessage
{
	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend((TArg0)args[0], (TArg1)args[1], (TArg2)args[2]);
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		m_innerMessage.AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		m_innerMessage.RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2);
	public delegate void HandlerAction(TArg0 arg0, TArg1 arg1, TArg2 arg2);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			convertedHandlerActions[handler] = ConvertHandlerAction(handler);
		}

		m_innerMessage.AddHandler(convertedHandlerActions[handler]);
	}

	public void RemoveAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.RemoveHandler(convertedHandlerActions[handler]);
		convertedHandlerActions.Remove(handler);
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

		m_innerMessage.RemoveHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{m_arg0, m_arg1, m_arg2 });
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2);
	}

	private Func<IEnumerator> ConvertHandlerAction(HandlerAction handler)
	{
		return () => WrapAction(handler);
	}

	private IEnumerator WrapAction(HandlerAction handler)
	{
		handler(m_arg0, m_arg1, m_arg2);
		yield break;
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
	private static readonly Dictionary<HandlerAction,Func<IEnumerator>> convertedHandlerActions = new Dictionary<HandlerAction, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2, TArg3> : IGenericMessage
{
	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend((TArg0)args[0], (TArg1)args[1], (TArg2)args[2], (TArg3)args[3]);
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		m_innerMessage.AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		m_innerMessage.RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
	public delegate void HandlerAction(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
        m_arg3 = arg3;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			convertedHandlerActions[handler] = ConvertHandlerAction(handler);
		}

		m_innerMessage.AddHandler(convertedHandlerActions[handler]);
	}

	public void RemoveAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.RemoveHandler(convertedHandlerActions[handler]);
		convertedHandlerActions.Remove(handler);
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

		m_innerMessage.RemoveHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{m_arg0, m_arg1, m_arg2, m_arg3 });
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2, m_arg3);
	}

	private Func<IEnumerator> ConvertHandlerAction(HandlerAction handler)
	{
		return () => WrapAction(handler);
	}

	private IEnumerator WrapAction(HandlerAction handler)
	{
		handler(m_arg0, m_arg1, m_arg2, m_arg3);
		yield break;
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
    private TArg3 m_arg3;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
	private static readonly Dictionary<HandlerAction,Func<IEnumerator>> convertedHandlerActions = new Dictionary<HandlerAction, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}
public class Message<TArg0, TArg1, TArg2, TArg3, TArg4> : IGenericMessage
{
	#region IGenericMessage implementation

	public IEnumerator WaitSendGeneric (object[] args)
	{
		return WaitSend((TArg0)args[0], (TArg1)args[1], (TArg2)args[2], (TArg3)args[3], (TArg4)args[4]);
	}

	public void AddHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			convertedGenericHandlers[handler] = convertGenericHandler(handler);

		m_innerMessage.AddHandler(convertedGenericHandlers[handler]);
	}

	public void RemoveHandlerGeneric (Func<object[], IEnumerator> handler)
	{
		if(!convertedGenericHandlers.ContainsKey(handler))
			return;

		m_innerMessage.RemoveHandler(convertedGenericHandlers[handler]);
		convertedGenericHandlers.Remove(handler);
	}

	#endregion

	public delegate IEnumerator Handler(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
	public delegate void HandlerAction(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
	
	public IEnumerator WaitSend(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
	{
        m_arg0 = arg0;
        m_arg1 = arg1;
        m_arg2 = arg2;
        m_arg3 = arg3;
        m_arg4 = arg4;
		yield return new Routine(m_innerMessage.WaitSend());
	}

	public void AddAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			convertedHandlerActions[handler] = ConvertHandlerAction(handler);
		}

		m_innerMessage.AddHandler(convertedHandlerActions[handler]);
	}

	public void RemoveAction(HandlerAction handler)
	{
		if(!convertedHandlerActions.ContainsKey(handler))
		{ 
			return;
		}

		m_innerMessage.RemoveHandler(convertedHandlerActions[handler]);
		convertedHandlerActions.Remove(handler);
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

		m_innerMessage.RemoveHandler(convertedHandlers[handler]);
		convertedHandlers.Remove(handler);
	}
	
	Func<IEnumerator> convertGenericHandler(Func<object[], IEnumerator> handler)
	{
		return () => wrapGenericHandler(handler);
	}

	IEnumerator wrapGenericHandler(Func<object[], IEnumerator> handler)
	{
		return handler(new Object[]{m_arg0, m_arg1, m_arg2, m_arg3, m_arg4 });
	}

	private Func<IEnumerator> ConvertHandler(Handler handler)
	{
		return () => handler(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
	}

	private Func<IEnumerator> ConvertHandlerAction(HandlerAction handler)
	{
		return () => WrapAction(handler);
	}

	private IEnumerator WrapAction(HandlerAction handler)
	{
		handler(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
		yield break;
	}

    private TArg0 m_arg0;
    private TArg1 m_arg1;
    private TArg2 m_arg2;
    private TArg3 m_arg3;
    private TArg4 m_arg4;
	private readonly Message m_innerMessage = new Message();
	private static readonly Dictionary<Handler,Func<IEnumerator>> convertedHandlers = new Dictionary<Handler, Func<IEnumerator>>();
	private static readonly Dictionary<HandlerAction,Func<IEnumerator>> convertedHandlerActions = new Dictionary<HandlerAction, Func<IEnumerator>>();
	private Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>> convertedGenericHandlers = new Dictionary<Func<object[], IEnumerator>, Func<IEnumerator>>();
}