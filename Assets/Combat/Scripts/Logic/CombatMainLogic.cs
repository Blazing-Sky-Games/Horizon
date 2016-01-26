using System.Collections;
using System.Collections.Generic;

public class CombatMainLogic : ViewLogic<CombatLogicData>
{
	//supplyed in Editor
	public CombatMainLogic(CombatLogicData Data)
		: base(Data)
	{
		//TODO creat turn order
		//m_turnOrder = new TurnOrder(Data);

		turnOrder = ServiceLocator.GetService<TurnOrder>();
		factionService = ServiceLocator.GetService<FactionService>();

		//creat the actors that will be playing
		//Horizon.Combat.Logic.Globals.SetFactionLeader(Faction.Player, new Actor());
		factionService.Dereference().SetFactionLeader(Faction.Player, new AIActor(Faction.Player));
		factionService.Dereference().SetFactionLeader(Faction.AI, new AIActor(Faction.AI));

		CoroutineService.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		//LogManager.NewCombatLog();
		//LogManager.Log("begin combat", LogDestination.Screen);

		while(true)
		{
			Actor FactionLeader = factionService.Dereference().GetFactionLeader(turnOrder.Dereference().ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while(FactionLeader.CanTakeAction)
			{
				if(!turnOrder.Dereference().ActiveUnit.CanTakeAction)
				{
					//decide for the actor, they cant do anything this turn
					yield return new Routine(FactionLeader.WaitPassTurn());
				}
				else
				{
					//tell the actor to decide what to do
					FactionLeader.ActionDecidedMessage.AddHandler(HandleActionDecided);
					CoroutineService.StartCoroutine(FactionLeader.WaitDecideAction());
					FactionLeader.ActionDecidedMessage.RemoveHandler(HandleActionDecided);
				}
			}

			//advance the turn order
			yield return new Routine(turnOrder.Dereference().WaitAdvance());
			//yield return new Routine(TurnBasedEffectManager.WaitUpdateTurnBasedEffects()); TODO
		}
	}

	private IEnumerator HandleActionDecided (IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	private WeakReference<FactionService> factionService;
	private WeakReference<TurnOrder> turnOrder;
}
