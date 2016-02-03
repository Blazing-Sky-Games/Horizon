using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatLogic
{
	public readonly Message CombatStarted = new Message();
	public readonly Message<Faction> CombatOver = new Message<Faction>();

	public CombatLogic(CombatScenario Data)
	{
		turnOrderService = ServiceLocator.GetService<ITurnOrderService>();
		factionService = ServiceLocator.GetService<IFactionService>();
		coroutineService = ServiceLocator.GetService<ICoroutineService>();
		enduringEffectService = ServiceLocator.GetService<IEnduringEffectService>();
		unitService = ServiceLocator.GetService<IUnitService>();

		//create the units in the combat scenrios
		var createdUnitsIds = unitService.CreateUnits(Data.Units);
		turnOrderService.SetOrder(createdUnitsIds);

		factionService.SetOpposingFaction(Faction.Player, Faction.AI);
		factionService.SetOpposingFaction(Faction.AI, Faction.Player);

		//create the actors that will be playing
		//factionService.SetFactionLeader(Faction.Player, new Actor(Faction.Player));
		factionService.SetFactionLeader(Faction.Player, new AIActor(Faction.Player, "player"));
		factionService.SetFactionLeader(Faction.AI, new AIActor(Faction.AI, "ai"));
	}

	public void Launch()
	{
		coroutineService.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		ServiceLocator.GetService<ILoggingService>().Log("combat Started");
		yield return new Routine(CombatStarted.WaitSend());

		while(true)
		{
			Actor FactionLeader = turnOrderService.ActiveUnit.Faction.GetLeader();

			FactionLeader.ResetCanTakeAction();

			//update turn based effects targeting the active unit
			List<TurnBasedEffect> tb = enduringEffectService.ActiveEffectsOfType<TurnBasedEffect>().ToList();
			List<TurnBasedEffect> turnBasedEffectsOnActiveUnit = tb.Where(effect => effect.Target.Equals(turnOrderService.ActiveUnit)).ToList();
			yield return new Routine(enduringEffectService.WaitUpdateEffects(turnBasedEffectsOnActiveUnit.Cast<EnduringEffect>()));

			while(FactionLeader.CanTakeAction)
			{
				if(turnOrderService.ActiveUnit.ActionPrevented)
				{
					//decide for the actor, they cant do anything this turn
					yield return new Routine(FactionLeader.WaitPassTurn());
				}
				else
				{
					//tell the actor to decide what to do
					FactionLeader.ActionDecidedMessage.AddHandler(WaitOnActionDecided);
					yield return new Routine(FactionLeader.WaitDecideAction());
					FactionLeader.ActionDecidedMessage.RemoveHandler(WaitOnActionDecided);
				}

				//check if the encounter is over
				//TODO make this generic (for a given win condition), and do not only check for wins here
				int aiUnits = unitService.UnitsOfFaction(Faction.AI).Count();
				int playerUnits = unitService.UnitsOfFaction(Faction.Player).Count();

				if(playerUnits == 0)
				{
					ServiceLocator.GetService<ILoggingService>().Log("lose");
					yield return new Routine(CombatOver.WaitSend(Faction.Player));
					yield break;
				}
				else if(aiUnits == 0)
				{
					ServiceLocator.GetService<ILoggingService>().Log("win");
					yield return new Routine(CombatOver.WaitSend(Faction.AI));
					yield break;
				}
			}

			//advance the turn order
			yield return new Routine(turnOrderService.WaitAdvance());
		}
	}

	private IEnumerator WaitOnActionDecided (IActorAction action)
	{
		yield return new Routine(action.WaitPerform());
	}

	private IFactionService factionService;
	private ITurnOrderService turnOrderService;
	private IUnitService unitService;
	private ICoroutineService coroutineService;
	private IEnduringEffectService enduringEffectService;
}
