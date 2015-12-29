using System.Collections;
using System.Collections.Generic;

//like a corutine, but it stops when a waitfornextturn is yeilded, and no longer updates, untill the next turn 
//TODO is there a better way to write this that doesnt duplicate so much code from corutine
using System.Linq;


public class TurnBassedEffect
{
	public TurnBassedEffect (IEnumerator routine)
	{
		m_callStack = new Stack<IEnumerator> ();
		m_callStack.Push (routine);
	}

	public bool Done 
	{
		get 
		{
			return m_callStack.Count == 0;
		}
	}

	public IEnumerator WaitUpdate ()
	{
		while(!Done)
		{
			//update the routine
			bool stackFrameRunning = m_callStack.Peek().MoveNext ();
			
			// this routine is finished running, return to caller
			if(!stackFrameRunning)
			{
				m_callStack.Pop();
				continue;
			}
			
			//the current routine is still running
			
			//what has the routine yielded
			object current = m_callStack.Peek ().Current;
			
			// if it is null, move along....
			if(current == null)
			{
				continue;
			}
			// if its a corutine, wait for it to finish (wait for frames)
			else if( current as Coroutine != null)
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
			//THIS I DIFFERNT FROM A REGULRE COROUTINE
			// wait until the next turn
			else if(current as WaitForNextTurn != null)
			{
				yield break;
			}
			// if its a ienumerator, step into it
			else if ( current as IEnumerator != null) 
			{
				IEnumerator cr = current as IEnumerator;
				m_callStack.Push (cr);
				continue;
			}
			// somthing else was yielded. wait a frame
			else
			{
				yield return 0;
			}
		}
	}

	Stack<IEnumerator> m_callStack;
}

public class TurnBasedEffectManager
{
	public static IEnumerator WaitStartTurnBasedEffect(IEnumerator effectRoutine)
	{
		TurnBassedEffect effect = new TurnBassedEffect(effectRoutine);
		effects.Add(effect);
		yield return CoroutineManager.Main.StartCoroutine(effect.WaitUpdate());
	}

	public static IEnumerator WaitUpdateTurnBasedEffects()
	{
		foreach(TurnBassedEffect effect in effects)
		{
			yield return CoroutineManager.Main.StartCoroutine(effect.WaitUpdate());
		}

		effects = effects.Where (x => x.Done == false).ToList();
	}

	private static List<TurnBassedEffect> effects = new List<TurnBassedEffect>();
}