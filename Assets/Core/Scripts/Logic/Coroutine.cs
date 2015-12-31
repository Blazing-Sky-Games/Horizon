using System.Collections;
using System.Collections.Generic;
using System;

// a currently running "async"(sorta) process, which represents another routine distinct from the starter of this Coroutine
// TODO need a way to handle exceptions at the call site of StartCoroutine
public class Coroutine
{
	public CoroutineException Error
	{
		get
		{
			return m_error;
		}
	}

	public Coroutine(IEnumerator routine, string meth = "", string file = "", int line = 0)
	{
		meth = meth == "" ? CallerInformation.MethodName : meth;
		file = file == "" ? CallerInformation.FilePath : file;
		line = line == 0 ? CallerInformation.LineNumber : line;
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
	
	//propogate exceptions up the call stack
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
				m_error = new CoroutineException(e, m_callStack);
				throw m_error;
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
					if(blockingRoutine.Error != null)
					{
						m_error = new CoroutineException(blockingRoutine.Error, m_callStack);
						throw m_error;
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
				m_error = new CoroutineException(new InvalidOperationException("yeilded object of invalid type. type was " + yielded.GetType().ToString()), m_callStack);
				throw m_error;
			}
		}
	}

	private CoroutineException m_error;
	private readonly Stack<Routine> m_callStack = new Stack<Routine>();
}