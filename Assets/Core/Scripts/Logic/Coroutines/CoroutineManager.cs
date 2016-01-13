using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO hmm....should the corutine manager be moved into view code
//TODO UGGGGHHHHHHH .... i really need to wrap everything in namespaces
public class CoroutineManager
{
	//globaly accsesable corutine manager ... TODO move this
	public static CoroutineManager Main
	{
		get
		{
			m_instance = m_instance ?? new CoroutineManager();

			return m_instance; 
		}
	}

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		Coroutine c = new Coroutine(routine, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber);
		m_routines.Add(c);
		
		return c;
	}
	
	public void UpdateCoroutines()
	{
		for(int i = 0; i < m_routines.Count; i++)
		{
			m_routines[i].Update();
		}
		
		m_routines = m_routines.Where(x => !x.Done).ToList();
	}
	
	private List<Coroutine> m_routines = new List<Coroutine>();
	private static CoroutineManager m_instance;
}
