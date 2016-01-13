using System.Collections;

//a syncronous process (which may take multiple frames)
public class Routine
{
	public readonly string CallerMethod;
	public readonly string CallerFile;
	public readonly int CallerLine;

	public Routine(IEnumerator routine)
	{
		m_routine = routine;
		CallerMethod = CallerInformation.MethodName;
		CallerFile = CallerInformation.FilePath;
		CallerLine = CallerInformation.LineNumber;
	}

	public Routine(IEnumerator routine, string method, string filePath, int lineNumber)
	{
		m_routine = routine;
		CallerMethod = method;
		CallerFile = filePath;
		CallerLine = lineNumber;
	}

	//TODO add error handling here
	//TODO add way to catach expetions from a yeild location (propogate exceptions up)
	//TODO better naming conventions
	public bool MoveNext()
	{
		return m_routine.MoveNext();
	}

	//TODO better naming conventions
	public object Current
	{
		get
		{
			return m_routine.Current;
		}
	}

	private readonly IEnumerator m_routine;
}
