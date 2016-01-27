using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Contexts;
using Core.Code.Services.LoggingService;


public class CoroutineService : ICoroutineService
{
	public void LoadService ()
	{
		loggingService = ServiceLocator.GetService<ILoggingService>();
	}

	public void UnloadService ()
	{
		loggingService = null;
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
				loggingService.ErrorLog.Log(m_routines[i].Error.ToString());
			}
		}
		
		m_routines = m_routines.Where(x => !x.Done).ToList();
	}

	private List<Coroutine> m_routines = new List<Coroutine>();

	private ILoggingService loggingService;
}
