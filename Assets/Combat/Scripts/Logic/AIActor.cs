using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class AIActor : Actor
{
	public AIActor(Faction faction)
	{
		m_faction = faction;
		turnOrder = ServiceUtility.GetServiceReference<TurnOrder>();
		factionService = ServiceUtility.GetServiceReference<FactionService>();
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		UnitLogic activeUnit = turnOrder.Dereference().ActiveUnit;

		if(factionService.Dereference().GetFactionLeader(activeUnit.Faction) != this)
		{
			throw new InvalidOperationException("AI can only decide action when it is its turn");
		}

		System.Random r = new System.Random();
		UnitLogic targetUnit = turnOrder.Dereference().Where(x => x.Faction != m_faction).OrderBy(x => r.NextDouble()).FirstOrDefault();
		if(targetUnit == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		UnitAbilityLogic SelectedAbility = activeUnit.Abilities.OrderBy(x => r.NextDouble()).FirstOrDefault();
		if(SelectedAbility == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		yield return new Routine(WaitUseUnitAbility(activeUnit, SelectedAbility, targetUnit));
	}

	private WeakReference<TurnOrder> turnOrder;

	private WeakReference<FactionService> factionService;

	private Faction m_faction;
}
