using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO UGGGGHHHHHHH .... i really need to wrap everything in namespaces
//TODO synchronozation between coroutines. i can think of some times it might be needed
using Core.Scripts.Contexts;


public class CoroutineService : ICoroutineService
{
	public void LoadService ()
	{
		throw new System.NotImplementedException();
	}

	public void UnloadService ()
	{
		throw new System.NotImplementedException();
	}

	public Coroutine StartCoroutine (IEnumerator routine)
	{
		Coroutine c = new Coroutine(routine, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber);
		m_routines.Add(c);
		
		return c;
	}

	public void UpdateCoroutines()
	{
		UpdateCoroutines<WaitForNextUpdate>();
	}

	public void UpdateCoroutines<YeildInstructionType> ()
		where YeildInstructionType : RoutineYieldInstruction, new()
	{
		for(int i = 0; i < m_routines.Count(); i++)
		{
			m_routines[i].Step(new YeildInstructionType());
			if(m_routines[i].HasError)
			{
				//log somewhere
			#if DEBUG
				UnityEngine.Debug.LogException(m_routines[i].Error);
			#endif
			}
		}
		
		m_routines = m_routines.Where(x => !x.Done).ToList();
	}

	private List<Coroutine> m_routines = new List<Coroutine>();
}
