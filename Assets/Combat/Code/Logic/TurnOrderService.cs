using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnOrderService : ITurnOrderService, IEnumerable<Unit>
{
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

	public void LoadService ()
	{
		throw new NotImplementedException();
	}

	public void UnloadService ()
	{
		throw new NotImplementedException();
	}

	public readonly Message AdvanceTurnOrderMessage = new Message();
	public readonly Message<bool> CombatEncounterOverMessage = new Message<bool>();
	public readonly Message<Unit> UnitKilledMessage = new Message<Unit>();

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

		yield return new Routine(AdvanceTurnOrderMessage.WaitSend());
	}

	private List<Unit> m_units;
	private int m_activeUnitIndex = 0;
}