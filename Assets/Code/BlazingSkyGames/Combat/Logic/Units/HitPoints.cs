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

using System.Collections;

public class HitPoints
{
	public readonly Message Hurt = new Message();
	public readonly Message MaxChanged = new Message();

	public readonly Message Zero = new Message();

	public int Current
	{
		get
		{
			return m_current;
		}
	}

	public int Max
	{
		get
		{
			return m_max;
		}
	}

	public HitPoints(int current, int maximum)
	{
		m_max = maximum;

		current = current < 0 ? 0 : current;
		current = current > maximum ? maximum : current;

		m_current = current;
	}

	public IEnumerator WaitHurt(int dmg)
	{
		m_current -= dmg;
		m_current = m_current < 0 ? 0 : Current;

		ServiceLocator.GetService<ILoggingService>().Log("unit hurt for  " + dmg);
		yield return new Routine(Hurt.WaitSend());

		if(m_current == 0)
		{
			//ServiceLocator.GetService<ILoggingService>().Log("unit health reached zero");
			yield return new Routine(Zero.WaitSend());
		}
	}

	public IEnumerator WaitSetMax(int max)
	{
		m_max = max;

		yield return new Routine(MaxChanged.WaitSend());

		if(m_current > m_max)
		{
			yield return new Routine(WaitHurt(m_current - m_max));
		}
	}

	private int m_current;
	private int m_max;
}