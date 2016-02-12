using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Coroutine : RoutineControlSignal
{
	public CoroutineException Error
	{
		get
		{
			return m_error;
		}
	}

	public bool HasError
	{
		get
		{
			return m_error != null;
		}
	}

	public bool CatchExceptions;

	public object Yielded
	{
		get
		{
			return m_callStack.Peek() != null ? m_callStack.Peek().Yielded : null;
		}
	}

	public Coroutine(IEnumerator routine, string meth = "", string file = "", int line = 0)
	{
		meth = meth == "" ? CallerInformation.MethodName : meth;
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
	
	//propogate exceptions up the call stack
	public void Step (RoutineYieldInstruction instruction)
	{
		while(!Done)
		{
			// the routine at the top of the call stack
			Routine topRoutine = m_callStack.Peek();
			
			//the routine is done
			if(topRoutine.Done)
			{
				Routine oldTop = m_callStack.Pop();
				if(oldTop.HasError)
				{
					oldTop.Error.CallStack.Add(oldTop);
					m_unwindQ.Enqueue(oldTop);

					if(m_callStack.Count > 0)
					{
						if(!oldTop.CatchExceptions)
						{
							//pass the error down the call stack
							m_callStack.Peek().Error = oldTop.Error;
						}
						else
						{
							//control passes down the stack, where the exception will be handled/ignored
							m_callStack.Peek().Step();
						}
					}
					else
					{
						// the corutine will exit with an error
						m_error = new CoroutineException(m_unwindQ.Peek().Error, new Stack<Routine>(m_unwindQ.ToArray().Reverse()));
					}
				}
				else if(m_callStack.Count > 0)
				{
					//control passes back down the ecall stack
					m_callStack.Peek().Step();
				}
				//control passes back down the call stack
				continue;
			}
			// the routine is not done and it yielded null
			// this means it either just started, or there was a yield break (a no-op)
			// in either case, continue the current routine
			else if(topRoutine.Yielded == null)
			{
				//continue the current routine
				topRoutine.Step();
				continue;
			}
			else if(topRoutine.Yielded is Coroutine)
			{
				var co = topRoutine.Yielded as Coroutine;
				if(!co.Done)
				{
					//this routine is blocked on a coroutine
					//TODO fix it so routines will start as soon as a corotine is finished, not when update is called again
					//If a program never yields a yield instruction, the coroutines should all run in a single frame
					break;
				}
				else if(co.HasError && !co.CatchExceptions)
				{
					// the corutine threw an exception
					topRoutine.Error = new RoutineException(co.Error);
					continue;
				}
				else
				{
					//the corutine finished. continue the current routine
					// if the corutine threw an exception, it will be in Error, and the current rotutine can do somthing with it
					topRoutine.Step();
					continue;
				}
			}
			// if its a routine, step into it
			else if(topRoutine.Yielded is Routine)
			{
				//push it on the stack and transfer control to it
				m_callStack.Push(topRoutine.Yielded as Routine);
				continue;
			} 
			//a yield instruction was yeilded. this means this coroutine is waiting for a relevent call to UpDateCoroutines
			//if we are updating this yeild instruction...
			else if(instruction != null && instruction.GetType() == topRoutine.Yielded.GetType())
			{
				topRoutine.Step();
				instruction = null;
				continue;
			}
			// we are stuck on a yield instruction that is not being updated right now
			else
			{
				break;
			}
		}
	}

	private CoroutineException m_error;
	private readonly Stack<Routine> m_callStack = new Stack<Routine>();
	private readonly Queue<Routine> m_unwindQ = new Queue<Routine>();
}