using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class AIActor : Actor
{
	public AIActor(string name, CombatLogic logic, Faction faction):base(name)
	{
		m_logic = logic;
		m_faction = faction;
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		UnitLogic activeUnit = m_logic.TurnOrder.ActiveUnit;

		if(m_logic.GetFactionLeader(activeUnit.Faction) != this)
		{
			throw new InvalidOperationException("AI can only decide action when it is its turn");
		}

		System.Random r = new System.Random();
		UnitLogic targetUnit = m_logic.TurnOrder.Where(x => x.Faction != m_faction).OrderBy(x => r.NextDouble()).FirstOrDefault();
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

	private CombatLogic m_logic;

	private Faction m_faction;
}
