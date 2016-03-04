/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

//a syncronous process (which may take multiple frames)
using System.Collections;
using System;

public class Routine : RoutineControlSignal
{
	public readonly string CallerMethod;
	public readonly string CallerFile;
	public readonly int CallerLine;

	public bool CatchExceptions;

	public virtual void Reset()
	{
		m_routine.Reset();
	}

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
	public override void Reset ()
	{
		base.Reset();
		m_hasResult = false;
	}

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
