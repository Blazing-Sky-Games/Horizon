using System.Collections;
using System.Collections.Generic;

// a currently running "async"(sorta) process
//TODO add debug info for stack traces that make sence
using System;

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
		string message = " Routine Call Stack (upto StartCorutine)\n";
		while(callStack.Count > 0)
		{
			Routine r = callStack.Pop();
			message += "\t" + r.callerMethod + " (at " + r.callerFilePath + ":" + r.callerLineNumber + ")\n";
		}
		m_message = message;
	} 
}

public class Routine
{
	public readonly string callerMethod;
	public readonly string callerFilePath;
	public readonly int callerLineNumber;

	public Routine(IEnumerator routine)
	{
		m_routine = routine;
		callerMethod = StackHelper.Caller__METHOD__;
		callerFilePath = StackHelper.Caller__FILE__;
		callerLineNumber = StackHelper.Caller__LINE__;
	}

	public Routine(IEnumerator routine, string method, string FilePath, int lineNumber)
	{
		m_routine = routine;
		callerMethod = method;
		callerFilePath = FilePath;
		callerLineNumber = lineNumber;
	}

	public bool MoveNext()
	{
		return m_routine.MoveNext();
	}

	public object Current
	{
		get
		{
			return m_routine.Current;
		}
	}


	IEnumerator m_routine;
}

public class Coroutine
{
	public CoroutineException error = null;

	public Coroutine(IEnumerator routine, string meth = "", string file = "", int line = 0)
	{
		meth = meth == "" ? StackHelper.Caller__METHOD__ : meth;
		file = file == "" ? StackHelper.Caller__FILE__ : file;
		line = line == 0 ? StackHelper.Caller__LINE__ : line;
		m_callStack.Push(new Routine(routine, meth, file, line));
		Update();
	}
	
	public bool Done
	{
		get
		{
			return m_callStack.Count == 0;
		}
	}
	
	public void Update()
	{
		while(!Done)
		{
			bool stackFrameRunning = false;

			//update the routine
			try
			{   
				stackFrameRunning = m_callStack.Peek().MoveNext();
			}
			catch(Exception e)
			{
				throw new CoroutineException(e, m_callStack);
			}
			
			// this routine is finished running, return to caller
			if(!stackFrameRunning)
			{
				m_callStack.Pop();
				continue;
			}
			
			//the current routine is still running
			
			//what has the routine yielded
			object yielded = m_callStack.Peek().Current;
			
			// if it is null, move along....
			if(yielded == null)
			{
				continue;
			}
			// if its a corutine, wait for it to finish (wait for frames)
			else if(yielded as Coroutine != null)
			{
				Coroutine blockingRoutine = yielded as Coroutine;
				//if it isnt done
				if(!blockingRoutine.Done)
				{
					break;
				}
				//if it is, keep going
				else
				{
					if(blockingRoutine.error != null)
					{
						throw new CoroutineException(blockingRoutine.error, m_callStack);
					}

					continue;
				}
			}
			// if its a ienumerator, step into it
			else if(yielded as Routine != null)
			{
				Routine cr = yielded as Routine;
				m_callStack.Push(cr);
				continue;
			}
			// int was yielded. wait a frame
			else if(yielded.GetType().IsAssignableFrom(typeof(int)))
			{
				break;
			}
			else
			{
				throw new CoroutineException(new InvalidOperationException("yeilded object of invalid type. type was " + yielded.GetType().ToString()), m_callStack);
			}
		}
	}
	
	private readonly Stack<Routine> m_callStack = new Stack<Routine>();
}