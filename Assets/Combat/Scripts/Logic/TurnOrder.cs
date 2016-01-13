using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


// list of units. the order in which unts take their turn
// also keeps track of units in general (death, etc)
public class TurnOrder : IEnumerable<UnitLogic>
{
	public readonly Message AdvanceTurnOrderMessage = new Message();
	public readonly Message<bool> CombatEncounterOverMessage = new Message<bool>();
	public readonly Message<UnitLogic> UnitKilledMessage = new Message<UnitLogic>();

	public TurnOrder(CombatLogicData scenario)
	{
		m_units = scenario.Units.Select(data => new UnitLogic(data)).ToList();

		foreach(UnitLogic unit in m_units)
		{
			//TODO hmm....fix it so we can remove this handler
			unit.Health.Zero.AddHandler(GetKillUnitHandler(unit));
		}
	}

	private Func<IEnumerator> GetKillUnitHandler(UnitLogic unit)
	{
		return () => WaitKillUnit(unit);
	}

	public UnitLogic ActiveUnit
	{
		get
		{
			return m_units[ActiveUnitIndex];
		}
	}

	public int ActiveUnitIndex
	{
		get
		{
			return m_activeUnitIndex;
		}
	}

	// remove a unit from the turn order
	public IEnumerator WaitKillUnit(UnitLogic killedUnit)
	{
		int killedIndex = m_units.IndexOf(killedUnit);

		if(killedIndex == -1)
		{
			throw new InvalidOperationException("cannot kill unit not in the turn order");
		}

		m_units.Remove(killedUnit); // need to handle the case where the active unit dies

		if(killedIndex > m_activeUnitIndex)
		{
			//dont need to do anything
		}
		else if(killedIndex < m_activeUnitIndex)
		{
			//adjust the active unit index because the active unit was shifeted back one
			m_activeUnitIndex--;
		}
		else if(killedIndex == m_activeUnitIndex)
		{
			//hmm......
		}

		// check if someone has won
		int numAI = 0;
		int numPlayer = 0;

		foreach(UnitLogic unit in m_units)
		{
			if(unit.Faction == Faction.AI)
			{
				numAI++;
			}
			else
			{
				numPlayer++;
			}
		}
		
		// send message that a unit has been killed
		yield return new Routine(UnitKilledMessage.WaitSend(killedUnit));

		if(numPlayer == 0)
		{
			yield return new Routine(CombatEncounterOverMessage.WaitSend(false));
		}
		else if(numAI == 0)
		{
			yield return new Routine(CombatEncounterOverMessage.WaitSend(true));
		}
	}

	//for IEnumerable<Unit>
	public IEnumerator<UnitLogic> GetEnumerator()
	{
		return m_units.GetEnumerator();
	}

	//for IEnumerable<Unit>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	//advance the turn order and wait for it to finish
	public IEnumerator WaitAdvance()
	{
		m_activeUnitIndex++;
		m_activeUnitIndex %= m_units.Count;

		yield return new Routine(AdvanceTurnOrderMessage.WaitSend());
	}

	private List<UnitLogic> m_units;
	private int m_activeUnitIndex = 0;
}
