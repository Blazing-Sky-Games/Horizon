using System.Collections;
using System.Collections.Generic;
using Combat.Code.Services.FactionService;
using Combat.Code.Services.TurnOrderService;
using Core.Scripts.Contexts;
using Combat.Code.Data.Logic.Effects;
using System.Linq;
using Core.Code.Services.LoggingService;
using Combat.Code.Extensions;

public class CombatMainLogic
{
	public CombatMainLogic(CombatScenario Data)
	{
		turnOrderService = ServiceLocator.GetService<ITurnOrderService>();
		factionService = ServiceLocator.GetService<IFactionService>();
		coroutineService = ServiceLocator.GetService<ICoroutineService>();
		enduringEffectService = ServiceLocator.GetService<IEnduringEffectService>();
		unitService = ServiceLocator.GetService<IUnitService>();
		loggingService = ServiceLocator.GetService<ILoggingService>();

		//create the units in the combat scenrios
		var createdUnitsIds = unitService.CreateUnits(Data.Units);
		turnOrderService.SetOrder(createdUnitsIds);

		//create the actors that will be playing
		//factionService.SetFactionLeader(Faction.Player, new Actor(Faction.Player));
		factionService.SetFactionLeader(Faction.Player, new AIActor(Faction.Player));
		factionService.SetFactionLeader(Faction.AI, new AIActor(Faction.AI));

		coroutineService.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		combatLog = loggingService.NewMultiLog(loggingService.NewLogFile("CombatLog"),loggingService.ScreenLog);
		combatLog.Log("begin combat");

		while(true)
		{
			Actor FactionLeader = unitService.GetUnit(turnOrderService.ActiveUnitId).Faction.GetLeader();

			FactionLeader.ResetCanTakeAction();

			//update turn based effects targeting the active unit
			IEnumerable<TurnBasedEffect> turnBasedEffectsOnActiveUnit = enduringEffectService.ActiveEffectsOfType<TurnBasedEffect>().Where(effect => effect.Target == turnOrderService.ActiveUnitId);
			foreach(TurnBasedEffect effect in turnBasedEffectsOnActiveUnit)
			{
				effect.OnNewTurn();
			}

			//update passive effects on the active unit
			IEnumerable<PassiveEffect> passiveEffectsOnActiveUnit = enduringEffectService.ActiveEffectsOfType<PassiveEffect>().Where(effect => effect.Target == turnOrderService.ActiveUnitId);
			foreach(PassiveEffect effect in passiveEffectsOnActiveUnit)
			{
				effect.OnNewTurn();
			}

			while(FactionLeader.CanTakeAction)
			{
				if(!unitService.GetUnit(turnOrderService.ActiveUnitId).CanTakeAction)
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
		}
	}

	private IEnumerator HandleActionDecided (IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	private IFactionService factionService;
	private ITurnOrderService turnOrderService;
	private IUnitService unitService;
	private ICoroutineService coroutineService;
	private IEnduringEffectService enduringEffectService;
	private ILoggingService loggingService;

	private ILog combatLog;
}
