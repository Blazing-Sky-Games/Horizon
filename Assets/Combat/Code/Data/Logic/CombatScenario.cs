using System.Collections.Generic;

[DataCatagory("Combat/Logic")]
public class CombatScenario : Data
{
	[UnityEngine.Tooltip("the turn order for this combat scenorio")]
	public List<UnitData> Units;
}

