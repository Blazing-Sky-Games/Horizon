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

		//creat the actors that will be playing
		//Horizon.Combat.Logic.Globals.SetFactionLeader(Faction.Player, new Actor());
		Horizon.Combat.Logic.Globals.SetFactionLeader(Faction.Player, new AIActor(Faction.Player));
		Horizon.Combat.Logic.Globals.SetFactionLeader(Faction.AI, new AIActor(Faction.AI));

		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		//LogManager.NewCombatLog();
		//LogManager.Log("begin combat", LogDestination.Screen);

		while(true)
		{
			Actor FactionLeader = Horizon.Combat.Logic.Globals.GetFactionLeader(Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while(FactionLeader.CanTakeAction)
			{
				if(!Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.CanTakeAction)
				{
					//decide for the actor, they cant do anything this turn
					yield return new Routine(FactionLeader.WaitPassTurn());
				}
				else
				{
					//tell the actor to decide what to do
					FactionLeader.ActionDecidedMessage.AddHandler(HandleActionDecided);
					Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(FactionLeader.WaitDecideAction());
					FactionLeader.ActionDecidedMessage.RemoveHandler(HandleActionDecided);
				}
			}

			//advance the turn order
			yield return new Routine(Horizon.Combat.Logic.Globals.turnOrder.WaitAdvance());
			yield return new Routine(TurnBasedEffectManager.WaitUpdateTurnBasedEffects());
		}
	}

	private IEnumerator HandleActionDecided (IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	public override void Destroy ()
	{
		//nothing
	}
}
