public interface IFactionService : IService
{
	Actor GetFactionLeader (Faction faction);

	void SetFactionLeader (Faction faction, Actor leader);

	Faction GetOpposingFaction (Faction faction);

	void SetOpposingFaction(Faction faction, Faction opposing);
}



