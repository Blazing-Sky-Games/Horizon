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
		factionService.SetFactionLeader(Faction.Player, new AIActor(Faction.Player));
		factionService.SetFactionLeader(Faction.AI, new AIActor(Faction.AI));
	}

	public void Start()
	{
		coroutineService.StartCoroutine(WaitCombatMain());
	}

	private IEnumerator WaitCombatMain ()
	{
		yield return new Routine(CombatStarted.WaitSend());

		while(true)
		{
			Actor FactionLeader = turnOrderService.ActiveUnit.Faction.GetLeader();

			FactionLeader.ResetCanTakeAction();

			//update turn based effects targeting the active unit
			IEnumerable<TurnBasedEffect> turnBasedEffectsOnActiveUnit = enduringEffectService.ActiveEffectsOfType<TurnBasedEffect>().Where(effect => effect.Target == turnOrderService.ActiveUnit);
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
				int aiUnits = unitService.UnitsOfFaction(Faction.AI).Count();
				int playerUnits = unitService.UnitsOfFaction(Faction.Player).Count();
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

	private ILog combatLog;
}
