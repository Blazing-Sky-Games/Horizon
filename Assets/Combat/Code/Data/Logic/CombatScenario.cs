using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[DataCatagory("Combat/Logic")]
public class CombatScenario : Data
{
	[UnityEngine.Tooltip("the turn order for this combat scenorio")]
	public List<UnitData> Units;
}

