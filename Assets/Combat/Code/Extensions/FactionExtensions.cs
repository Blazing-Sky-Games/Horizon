using System.Collections.Generic;

public static class FactionExtensions
{
	private static IUnitService unitService;
	private static IFactionService factionService;

	static FactionExtensions()
	{
		unitService = ServiceLocator.GetService<IUnitService>();
		factionService = ServiceLocator.GetService<IFactionService>();
	}

	public static IEnumerable<UnitId> GetUnits (this Faction faction)
	{
		return unitService.UnitsOfFaction(faction);
	}

	public static Actor GetLeader (this Faction faction)
	{
		return factionService.GetFactionLeader(faction);
	}
}