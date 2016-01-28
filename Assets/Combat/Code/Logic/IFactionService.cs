public interface IFactionService : IService
{
	Actor GetFactionLeader (Faction faction);

	Faction GetOpposingFaction (Faction faction);

	void SetFactionLeader (Faction faction, Actor leader);
}



