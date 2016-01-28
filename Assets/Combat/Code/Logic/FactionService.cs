using System;
using System.Collections.Generic;

public class FactionService : IFactionService
{
	public void LoadService ()
	{
		//nothing
	}

	public void UnloadService ()
	{
		//nothing
	}

	public Actor GetFactionLeader (Faction faction)
	{
		return factionLeaders[faction];
	}

	public void SetFactionLeader (Faction faction, Actor leader)
	{
		factionLeaders[faction] = leader;
	}

	public Faction GetOpposingFaction (Faction faction)
	{
		return OpposingFactions[faction];
	}

	public void SetOpposingFaction(Faction faction, Faction opposing)
	{
		OpposingFactions[faction] = opposing;
	}

	private readonly Dictionary<Faction,Faction> OpposingFactions = new Dictionary<Faction, Faction>();
	private readonly Dictionary<Faction,Actor> factionLeaders = new Dictionary<Faction, Actor>();
}