using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO UGGGGHHHHHHH .... i really need to wrap everything in namespaces
public class CoroutineManager
{
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
