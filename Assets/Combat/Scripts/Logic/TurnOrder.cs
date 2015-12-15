using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// list of units. the order in which unts take their turn
// also keeps track of units in general (death, etc)
public class TurnOrder : IEnumerable<Unit>
{
	public readonly MessageChannel AdvanceTurnOrderMessage = new MessageChannel ();
	public readonly MessageChannel<bool> CombatEncounterOverMessage = new MessageChannel<bool> (); 
	public readonly MessageChannel<Unit> UnitKilledMessage = new MessageChannel<Unit>();

	public TurnOrder(CombatScenario scenario)
	{
		m_units = scenario.Units;

		foreach (Unit unit in m_units)
		{
			unit.SetTurnOrder(this);
		}
	}

	public Unit ActiveUnit
	{
		get
		{
			return m_units [ActiveUnitIndex];
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
	public void UnitKilled(Unit killedUnit)
	{
		int killedIndex = m_units.IndexOf (killedUnit);
		m_units.Remove (killedUnit); // need to handle the case where the active unit dies

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
		
		foreach (Unit unit in m_units)
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
		
		if (numPlayer == 0)
		{
			CombatEncounterOverMessage.SendMessage (false); // TODO wait for CombatEncounterOverMessage
		}
		else if (numAI == 0)
		{
			CombatEncounterOverMessage.SendMessage (true); // // TODO wait for CombatEncounterOverMessage
		}

		// send message that a unit has been killed
		UnitKilledMessage.SendMessage(killedUnit); // TODO wait for UnitKilledMessage
	}

	//for IEnumerable<Unit>
	public IEnumerator<Unit> GetEnumerator()
	{
		return m_units.GetEnumerator ();
	}

	//for IEnumerable<Unit>
	IEnumerator IEnumerable.GetEnumerator ()
	{
		return GetEnumerator ();
	}

	//advance the turn order and wait for it to finish
	public Coroutine WaitAdvanceTurnOrder ()
	{
		m_activeUnitIndex++;
		m_activeUnitIndex %= m_units.Count;

		AdvanceTurnOrderMessage.SendMessage ();
		return AdvanceTurnOrderMessage.WaitTillMessageProcessed ();
	}

	private List<Unit> m_units;
	private int m_activeUnitIndex = 0;
}
