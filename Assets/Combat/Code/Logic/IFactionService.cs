using UnityEngine;
using System;
using System.Linq;
using System.Collections;

namespace Combat.Code.Services.FactionService
{
	public interface IFactionService : IService
	{
		Actor GetFactionLeader (Faction faction);
		Faction GetOpposingFaction (Faction faction);
		void SetFactionLeader (Faction faction, Actor leader);
	}
}


