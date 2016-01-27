using System;
using System.Collections.Generic;

namespace Combat.Code.Extensions
{
	public static class FactionExtensions
	{
		private static IUnitService unitService;

		static FactionExtensions()
		{
			unitService = ServiceLocator.GetService<IUnitService>();
		}

		public static IEnumerable<Unit> GetUnits(this Faction faction)
		{
			return unitService.UnitsOfFaction(faction);
		}
	}
}

