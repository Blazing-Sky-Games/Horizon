using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatScenario : ScriptableObject
{
	// the units, and the turn order
	public List<Unit> Units;

	// boiler plate funtion to deal with some unity weirdness
	// TODO will proabaly get removed
	public CombatScenario DeepCopy()
	{
		CombatScenario cs = UnityEngine.Object.Instantiate (this);

		List<Unit> newUnits = new List<Unit> ();
		foreach (Unit unit in Units)
		{
			newUnits.Add(unit.DeepCopy());
		}

		cs.Units = newUnits;

		return cs;
	}
}

