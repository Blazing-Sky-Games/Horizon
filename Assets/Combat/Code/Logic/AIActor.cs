using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using Combat.Code.Services.TurnOrderService;
using Combat.Code.Services.FactionService;
using Combat.Code.Extensions;

public class AIActor : Actor
{
	public AIActor(Faction faction)
	{
		m_faction = faction;
		turnOrderService = ServiceLocator.GetService<TurnOrder>();
		factionService = ServiceLocator.GetService<FactionService>();
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		Unit activeUnit = turnOrderService.ActiveUnit;

		if(factionService.GetFactionLeader(activeUnit.Faction) != this)
		{
			throw new InvalidOperationException("AI can only decide action when it is its turn");
		}
			
		Unit targetUnit = factionService.GetOpposingFaction(m_faction).GetUnits().RandomOrder().FirstOrDefault();
		if(targetUnit == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		UnitAbility SelectedAbility = activeUnit.Abilities.RandomOrder().FirstOrDefault();
		if(SelectedAbility == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		yield return new Routine(WaitUseUnitAbility(activeUnit, SelectedAbility, targetUnit));
	}

	private ITurnOrderService turnOrderService;

	private IUnitService unitService;

	private IFactionService factionService;

	private Faction m_faction;
}
