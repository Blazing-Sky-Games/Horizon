using System.Collections;
using System.Collections.Generic;

public class CombatLogic : DataDrivenLogic<CombatLogicData>
{
	//supplyed in Editor
	public CombatLogic(CombatLogicData data) : base(data)
	{
		m_turnOrder = new TurnOrder(Data);

		//creat the actors that will be playing
		//TODO better way to get actors
		//TODO actually, get rid of the whole actor thing, and just ask units what they want to do, not "actors"
		//m_factionLeaders[Faction.Player] = new Actor("player");
		m_factionLeaders[Faction.Player] = new AIActor("AI", this, Faction.Player); // use ai for player for testing
		m_factionLeaders[Faction.AI] = new AIActor("AI", this, Faction.AI);

		CoroutineManager.Main.StartCoroutine(WaitCombatMain());
	}

	public TurnOrder TurnOrder
	{
		get
		{
			return m_turnOrder;
		}
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
			Actor FactionLeader = GetFactionLeader(m_turnOrder.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while(FactionLeader.CanTakeAction)
			{
				if (!TurnOrder.ActiveUnit.CanTakeAction)
					//decide for the actor, they cant do anything this turn
					yield return new Routine (FactionLeader.WaitPassTurn ());
				else {
					//tell the actor to decide what to do
					CoroutineManager.Main.StartCoroutine (FactionLeader.WaitDecideAction ());

					//wait for the current actor to decide an action
					while (FactionLeader.ActionDecidedMessage.Idle) {
						yield return 0;
					}
				}
				//perform that action and wait for it to finish
				yield return new Routine(FactionLeader.ActionDecidedMessage.WaitHandleMessage(WaitHandleActionDecided));
			}

			//advance the turn order
			yield return new Routine(m_turnOrder.WaitAdvance());
			yield return new Routine(TurnBasedEffectManager.WaitUpdateTurnBasedEffects());
		}
	}

	private IEnumerator WaitHandleActionDecided(IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}
	
	private TurnOrder m_turnOrder;
	private readonly Dictionary<Faction, Actor> m_factionLeaders = new Dictionary<Faction, Actor>();
}
