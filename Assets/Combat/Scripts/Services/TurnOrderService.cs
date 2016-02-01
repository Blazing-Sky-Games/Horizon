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