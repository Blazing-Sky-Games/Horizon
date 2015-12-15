using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatScenario : ScriptableObject
{
	public List<Unit> Units;

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

