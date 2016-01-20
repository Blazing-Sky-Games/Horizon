using System.Collections.Generic;

namespace Horizon.Combat.Logic
{
	public static class Globals
	{
		public static TurnOrder turnOrder;

		public static Actor GetFactionLeader(Faction faction)
		{
			return FactionLeaders[faction];
		}

		public static void SetFactionLeader(Faction faction, Actor leader)
		{
			FactionLeaders[faction] = leader;
		}

		private static Dictionary<Faction,Actor> FactionLeaders = new Dictionary<Faction, Actor>();

		public static Message<UnitLogic> UnitKilled = new Message<UnitLogic>();
		public static Message<bool> CombatEncounterOver = new Message<bool>();
	}
}
