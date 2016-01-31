using System.Collections.Generic;

interface IUnitService : IService
{
	Message<Unit> UnitDied { get;}

	IEnumerable<Unit> UnitsOfFaction (Faction faction);

	IEnumerable<Unit> CreateUnits (IEnumerable<UnitData> units);
	Unit CreateUnit (UnitData unit);
}

