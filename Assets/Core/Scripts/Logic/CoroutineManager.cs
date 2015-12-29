using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CoroutineManager
{
	public static CoroutineManager Main
	{
		get
		{
			m_instance = m_instance ?? new CoroutineManager();

			return m_instance; 
		}
	}

	private static CoroutineManager m_instance;

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		Coroutine c = new Coroutine(routine);
		routines.Add(c);
		
		return c;
	}
	
	public void UpdateCoroutines()
	{
		for(int i = 0; i < routines.Count; i++)
		{
			routines[i].Update();
		}
		
		routines = routines.Where(x => x.Done == false).ToList();
	}
	
	List<Coroutine> routines = new List<Coroutine>();
}

// a currently running "async"(sorta) process
public class Coroutine
{
	public Coroutine(IEnumerator routine)
	{
		m_callStack = new Stack<IEnumerator>();
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
				if(blockingRoutine.Done == false)
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
			else if(current as IEnumerator != null)
			{
				IEnumerator cr = current as IEnumerator;
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
	
	Stack<IEnumerator> m_callStack;
}