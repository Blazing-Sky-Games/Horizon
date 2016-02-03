using System;
using System.Collections.Generic;

public class CoroutineException : Exception
{
	public override string Message
	{
		get
		{
			return m_message;
		}
	}

	private string m_message;

	public CoroutineException(Exception e, Stack<Routine> callStack) : base("",e)
	{
		string message = "\nRoutine Call Stack (up to call to StartCorutine)\n";
		while(callStack.Count > 0)
		{
			Routine r = callStack.Pop();
			message += ":    " + r.CallerMethod + " (at " + r.CallerFile + ":" + r.CallerLine + ")\n";
		}
		m_message = message;
	} 
}