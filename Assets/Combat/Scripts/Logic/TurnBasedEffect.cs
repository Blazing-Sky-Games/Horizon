using System.Collections;
using System.Collections.Generic;

//like a corutine, but it stops when a waitfornextturn is yeilded, and no longer updates, untill the next turn 
//TODO is there a better way to write this that doesnt duplicate so much code from corutine
using System;


//TODO make turn bassed effect less general. so it exposes needed information (turns left, severity, ability to cancel...)
public class TurnBasedEffect
{
	public TurnBasedEffect(IEnumerator routine, string meth = "", string file = "", int line = 0)
	{
		meth = file == "" ? CallerInformation.MethodName : meth;
		file = file == "" ? CallerInformation.FilePath : file;
		line = line == 0 ? CallerInformation.LineNumber : line;
		m_callStack.Push(new Routine(routine, meth, file, line));
	}

	public bool Done
	{
		get
		{
			return m_callStack.Count == 0;
		}
	}

	public IEnumerator WaitUpdate()
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
			object current = m_callStack.Peek().Current;
			
			// if it is null, move along....
			if(current == null)
			{
				continue;
			}
			// if its a corutine, wait for it to finish (wait for frames)
			else if(current as Coroutine != null)
			{
				Coroutine blockingRoutine = current as Coroutine;
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
						throw new CoroutineException(blockingRoutine.Error, m_callStack);
					}

					continue;
				}
			}
			//THIS I DIFFERNT FROM A REGULRE COROUTINE
			// wait until the next turn
			else if(current.GetType() == typeof(WaitForNextTurn))
			{
				yield break;
			}
			// if its a ienumerator, step into it
			else if(current as Routine != null)
			{
				Routine cr = current as Routine;
				m_callStack.Push(cr);
				continue;
			}
			// somthing else was yielded. wait a frame
			// int was yielded. wait a frame
			else if(current.GetType().IsAssignableFrom(typeof(int)))
			{
				yield return 0;
			}
			else
			{
				throw new CoroutineException(new InvalidOperationException("yeilded object of invalid type. type was " + current.GetType().ToString()), m_callStack);
			}
		}
	}

	private Stack<Routine> m_callStack = new Stack<Routine>();
}
