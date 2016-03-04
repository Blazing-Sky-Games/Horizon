/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
