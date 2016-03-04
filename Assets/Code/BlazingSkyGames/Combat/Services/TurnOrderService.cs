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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnOrderService : Service, ITurnOrderService, IEnumerable<Unit>
{
	private readonly Message m_orderChanged = new Message();

	public Message OrderChanged
	{
		get
		{
			return m_orderChanged;
		}
	}

	public override IEnumerator WaitLoadService ()
	{
		ServiceLocator.GetService<IUnitService>().UnitDied.AddHandler(WaitOnUnitDied);
		yield break;
	}

	private IEnumerator WaitOnUnitDied (Unit unit)
	{
		int indexOf = m_units.IndexOf(unit);

		if(indexOf == m_activeUnitIndex)
		{
			//umm ... what do we do in this case
		}
		else if(indexOf < m_activeUnitIndex)
		{
			// we dont have to do anything
		}
		else if(indexOf > m_activeUnitIndex)
		{
			//reduce the active unit index by one
			m_activeUnitIndex--;
		}

		m_units.Remove(unit);
		ServiceLocator.GetService<ILoggingService>().Log("turn order changed");
		yield return new Routine(OrderChanged.WaitSend());
	}

	public void SetOrder (IEnumerable<Unit> units)
	{
		m_units = units.ToList();
	}

	public Unit ActiveUnit
	{
		get
		{
			return m_units[ActiveUnitIndex];
		}
	}

	public Message TurnOrderAdvanced
	{
		get
		{
			return m_turnOrderAdvanced;
		}
	}

	private readonly Message m_turnOrderAdvanced = new Message();

	public int ActiveUnitIndex
	{
		get
		{
			return m_activeUnitIndex;
		}
	}

	//for IEnumerable<Unit>
	public IEnumerator<Unit> GetEnumerator ()
	{
		return m_units.GetEnumerator();
	}

	//for IEnumerable<Unit>
	IEnumerator IEnumerable.GetEnumerator ()
	{
		return GetEnumerator();
	}

	public IEnumerator WaitAdvance ()
	{
		m_activeUnitIndex++;
		m_activeUnitIndex %= m_units.Count;
		ServiceLocator.GetService<ILoggingService>().Log("turn order advanced");
		yield return new Routine(m_turnOrderAdvanced.WaitSend());
	}

	private List<Unit> m_units;
	private int m_activeUnitIndex = 0;
}