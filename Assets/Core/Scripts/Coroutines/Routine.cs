//a syncronous process (which may take multiple frames)
using System.Collections;
using System;


public class Routine : RoutineControlSignal
{
	public readonly string CallerMethod;
	public readonly string CallerFile;
	public readonly int CallerLine;

	public bool CatchExceptions;

	public RoutineException Error
	{
		get
		{
			return m_error;
		}
		set
		{
			m_error = value;
			if(HasError)
				m_done = true;
		}
	}

	public bool HasError
	{
		get{ return m_error != null; }
	}

	public Routine(IEnumerator routine) 
		: this(routine, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber)
	{
	}

	public Routine(IEnumerator routine, string method, string filePath, int lineNumber)
	{
		m_routine = routine;
		CallerMethod = method;
		CallerFile = filePath;
		CallerLine = lineNumber;
	}

	public virtual void Step ()
	{
		try
		{
			m_done = !m_routine.MoveNext();
		}
		catch(Exception e)
		{
			m_error = new RoutineException(e);
			m_done = true;
			return;
		}

		if(Yielded != null && !typeof(RoutineControlSignal).IsAssignableFrom(Yielded.GetType()))
		{
			Error = new RoutineException(new InvalidOperationException("you can only yield a RoutineControlSignal from a routine"));
		}
	}

	public bool Done
	{
		get
		{
			return m_done;
		}
	}

	public virtual object Yielded
	{
		get
		{
			return m_routine.Current;
		}
	}

	protected readonly IEnumerator m_routine;
	protected bool m_done;
	private RoutineException m_error;
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

	public Routine(IEnumerator routine)
		: base(routine)
	{
	}

	public Routine(IEnumerator routine, string method, string filePath, int lineNumber)
		: base(routine, method, filePath, lineNumber)
	{
	}

	public override void Step ()
	{
		try
		{
			m_done = !m_routine.MoveNext();
		}
		catch(Exception e)
		{
			Error = new RoutineException(e);
		}

		if(typeof(T).IsAssignableFrom(Yielded.GetType()))
		{
			m_result = (T)Yielded;
			m_hasResult = true;
			m_done = true;
		}
		else if(Yielded != null && !typeof(RoutineControlSignal).IsAssignableFrom(Yielded.GetType()))
		{
			Error = new RoutineException(new InvalidOperationException("you can only yield a RoutineControlSignal from a routine"));
		}
	}

	private T m_result;
	private bool m_hasResult;
}
