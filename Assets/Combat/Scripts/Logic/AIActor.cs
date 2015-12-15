using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class AIActor : Actor
{
	public AIActor(string name,CombatLogic logic):base(name)
	{
		m_logic = logic;
	}

	public override void DecideAction ()
	{
		Unit activeUnit = m_logic.TurnOrder.ActiveUnit;

		if(m_logic.GetFactionLeader(activeUnit.Faction) != this)
			throw new InvalidOperationException("AI can only decide action when it is its turn");

		System.Random r = new System.Random ();
		Unit targetUnit = m_logic.TurnOrder.Where (x => x.Faction == Faction.Player).OrderBy(x => r.NextDouble ()).First ();
		UnitAbility SelectedAbility = activeUnit.abilities.OrderBy (x => r.NextDouble ()).First ();

		UseUnitAbility (activeUnit, SelectedAbility, targetUnit);
	}

	private CombatLogic m_logic;
}
