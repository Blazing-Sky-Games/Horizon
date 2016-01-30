using System.Collections.Generic;

interface IUnitService : IService
{
	IEnumerable<Unit> UnitsOfFaction (Faction faction);

	IEnumerable<Unit> CreateUnits (IEnumerable<UnitData> units);
	Unit CreateUnit (UnitData unit);
}

