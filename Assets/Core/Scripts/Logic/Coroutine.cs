using System.Collections;
using System.Collections.Generic;

// a currently running "async"(sorta) process
//TODO add debug info for stack traces that make sence
public class Coroutine
{
	public Coroutine(IEnumerator routine)
	{
		m_callStack.Push(routine);
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
			//update the routine
			bool stackFrameRunning = m_callStack.Peek().MoveNext();
			
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
					continue;
				}
			}
			// if its a ienumerator, step into it
			else if(yielded as IEnumerator != null)
			{
				IEnumerator cr = yielded as IEnumerator;
				m_callStack.Push(cr);
				continue;
			}
			// somthing else was yielded. wait a frame
			else
			{
				break;
			}
		}
	}
	
	private readonly Stack<IEnumerator> m_callStack = new Stack<IEnumerator>();
}