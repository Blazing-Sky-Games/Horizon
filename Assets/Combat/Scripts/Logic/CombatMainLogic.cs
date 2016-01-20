using System.Collections;
using System.Collections.Generic;

public class CombatMainLogic : ViewLogic<CombatLogicData>
{
	//supplyed in Editor
	public CombatMainLogic(CombatLogicData Data) : base(Data)
	{
		//TODO creat turn order
		//m_turnOrder = new TurnOrder(Data);

		//creat the actors that will be playing
		//TODO better way to get actors
		//m_factionLeaders[Faction.Player] = new Actor("player");
		m_factionLeaders[Faction.Player] = new AIActor(Faction.Player); // use ai for player for testing
		m_factionLeaders[Faction.AI] = new AIActor(Faction.AI);

		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(WaitCombatMain());
	}

	//get the Actor Associated with a faction
	public Actor GetFactionLeader(Faction faction)
	{
		return m_factionLeaders[faction];
	}

	private IEnumerator WaitCombatMain()
	{
		//LogManager.NewCombatLog();
		//LogManager.Log("begin combat", LogDestination.Screen);

		while(true)
		{
			Actor FactionLeader = GetFactionLeader(Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while(FactionLeader.CanTakeAction)
			{
				if (!Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.CanTakeAction)
					//decide for the actor, they cant do anything this turn
					yield return new Routine (FactionLeader.WaitPassTurn ());
				else {
					//tell the actor to decide what to do
					Horizon.Core.Logic.Globals.Coroutines.StartCoroutine (FactionLeader.WaitDecideAction ());

					//wait for the current actor to decide an action
					while (FactionLeader.ActionDecidedMessage.Idle) {
						yield return 0;
					}
				}
				//perform that action and wait for it to finish
				yield return new Routine(FactionLeader.ActionDecidedMessage.WaitHandleMessage(WaitHandleActionDecided));
			}

			//advance the turn order
			yield return new Routine(Horizon.Combat.Logic.Globals.turnOrder.WaitAdvance());
			yield return new Routine(TurnBasedEffectManager.WaitUpdateTurnBasedEffects());
		}
	}

	private IEnumerator WaitHandleActionDecided(IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	public override void Destroy ()
	{
		//nothing
	}

	private readonly Dictionary<Faction, Actor> m_factionLeaders = new Dictionary<Faction, Actor>();
}
