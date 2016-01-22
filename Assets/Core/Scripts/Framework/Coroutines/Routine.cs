using System.Collections;

//a syncronous process (which may take multiple frames)
using System;


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
	public virtual bool Step()
	{
		return m_routine.MoveNext();
	}

	//TODO better naming conventions
	public virtual object Yielded
	{
		get
		{
			return m_routine.Current;
		}
	}

	protected readonly IEnumerator m_routine;
}

//a syncronous process (which may take multiple frames) and returns a value
public class Routine<T> : Routine
{
	public T Result
	{
		get
		{
			if(!HasResult)
				throw new InvalidOperationException("This routine has not returned a result!");

			return m_result;
		}
	}

	public bool HasResult
	{
		get
		{
			return m_hasResult;
		}
	}

	public Routine(IEnumerator routine) : base(routine){}

	public Routine(IEnumerator routine, string method, string filePath, int lineNumber) : base(routine, method, filePath, lineNumber) {}

	public override bool Step ()
	{
		bool done = base.Step();

		if(typeof(T).IsAssignableFrom(Yielded.GetType()))
		{
			m_result = (T)Yielded;
			m_hasResult = true;
			return false;
		}

		return done;
	}

	private T m_result;
	private bool m_hasResult;
}
