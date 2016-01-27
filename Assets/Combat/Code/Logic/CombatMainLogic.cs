using System.Collections;
using System.Collections.Generic;
using Combat.Code.Services.FactionService;
using Combat.Code.Services.TurnOrderService;
using Core.Scripts.Contexts;

public class CombatMainLogic
{
	//supplyed in Editor
	public CombatMainLogic(CombatLogicData Data)
	{
		//TODO creat turn order
		//m_turnOrder = new TurnOrder(Data);

		turnOrderService = ServiceLocator.GetService<ITurnOrderService>();
		factionService = ServiceLocator.GetService<IFactionService>();
		coroutineService = ServiceLocator.GetService<ICoroutineService>();

		//creat the actors that will be playing
		//Horizon.Combat.Logic.Globals.SetFactionLeader(Faction.Player, new Actor());
		factionService.SetFactionLeader(Faction.Player, new AIActor(Faction.Player));
		factionService.SetFactionLeader(Faction.AI, new AIActor(Faction.AI));

		coroutineService.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		//LogManager.NewCombatLog();
		//LogManager.Log("begin combat", LogDestination.Screen);

		while(true)
		{
			Actor FactionLeader = factionService.GetFactionLeader(turnOrderService.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while(FactionLeader.CanTakeAction)
			{
				if(!turnOrderService.ActiveUnit.CanTakeAction)
				{
					//decide for the actor, they cant do anything this turn
					yield return new Routine(FactionLeader.WaitPassTurn());
				}
				else
				{
					//tell the actor to decide what to do
					FactionLeader.ActionDecidedMessage.AddHandler(HandleActionDecided);
					coroutineService.StartCoroutine(FactionLeader.WaitDecideAction());
					FactionLeader.ActionDecidedMessage.RemoveHandler(HandleActionDecided);
				}
			}

			//advance the turn order
			yield return new Routine(turnOrderService.WaitAdvance());
			//yield return new Routine(TurnBasedEffectManager.WaitUpdateTurnBasedEffects()); TODO
		}
	}

	private IEnumerator HandleActionDecided (IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	private IFactionService factionService;
	private ITurnOrderService turnOrderService;
	private ICoroutineService coroutineService;
}
