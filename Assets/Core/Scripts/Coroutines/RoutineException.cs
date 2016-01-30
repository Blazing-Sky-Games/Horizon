using System;
using System.Collections.Generic;

public class RoutineException : Exception
{
	public override string Message
	{
		get
		{
			string msg = "Exception in routine\n";

			foreach(var rot in CallStack)
			{
				msg += rot.CallerMethod + " (at " + rot.CallerFile + ":" + rot.CallerLine + ")\n";
			}

			return msg;
		}
	}

	public RoutineException(Exception e):base("",e){
	}

	public readonly List<Routine> CallStack = new List<Routine>();
}
