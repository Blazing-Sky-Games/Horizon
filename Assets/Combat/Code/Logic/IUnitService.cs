using System.Collections.Generic;

interface IUnitService : IService
{
	IEnumerable<UnitId> UnitsOfFaction (Faction faction);

	Unit GetUnit(UnitId id);

	IEnumerable<UnitId> CreateUnits (IEnumerable<UnitData> units);
	UnitId CreateUnit (UnitData unit);
}

