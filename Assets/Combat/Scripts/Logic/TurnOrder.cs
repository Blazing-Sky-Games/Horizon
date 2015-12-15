using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TurnOrder : IEnumerable<Unit>
{
	IEnumerator IEnumerable.GetEnumerator ()
	{
		return GetEnumerator ();
	}

	public readonly MessageChannel AdvanceTurnOrderMessage = new MessageChannel ();
	public readonly MessageChannel<bool> CombatEncounterOverMessage = new MessageChannel<bool> (); 
	public readonly MessageChannel<Unit> UnitKilledMessage = new MessageChannel<Unit>();

	public TurnOrder(CombatScenario scenario)
	{
		units = scenario.Units;

		foreach (Unit unit in units)
		{
			unit.SetTurnOrder(this);
		}
	}

	public Unit ActiveUnit
	{
		get
		{
			return units [ActiveUnitIndex];
		}
	}

	public int ActiveUnitIndex
	{
		get
		{
			return index;
		}
	}

	public void UnitKilled(Unit killedUnit)
	{
		int killedIndex = units.IndexOf (killedUnit);
		units.Remove (killedUnit); // need to handle the case where the active unit dies

		if(killedIndex > index)
		{
			//dont need to do anything
		}
		else if(killedIndex < index)
		{
			//adjust the active unit index because the active unit was shifeted back one
			index--;
		}
		else if(killedIndex == index)
		{
			//hmm......
		}

		int numAI = 0;
		int numPlayer = 0;
		
		foreach (Unit unit in units)
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
			CombatEncounterOverMessage.SendMessage (false);
		}
		else if (numAI == 0)
		{
			CombatEncounterOverMessage.SendMessage (true);
		}

		UnitKilledMessage.SendMessage(killedUnit);
	}

	public IEnumerator<Unit> GetEnumerator()
	{
		return units.GetEnumerator ();
	}

	public Coroutine WaitAdvanceTurnOrder ()
	{
		index++;
		index %= units.Count;

		AdvanceTurnOrderMessage.SendMessage ();
		return AdvanceTurnOrderMessage.WaitTillMessageProcessed ();
	}

	private List<Unit> units;
	private int index = 0;
}
