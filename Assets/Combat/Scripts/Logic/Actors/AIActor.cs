using System;
using System.Collections;
using System.Linq;

public class AIActor : Actor
{
	public AIActor(Faction faction)
	{
		m_faction = faction;
		turnOrderService = ServiceLocator.GetService<ITurnOrderService>();
		factionService = ServiceLocator.GetService<IFactionService>();
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		Unit activeUnit = turnOrderService.ActiveUnit;

		if(activeUnit.Faction.GetLeader() != this)
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

		yield return new Routine(WaitUseUnitAbility(turnOrderService.ActiveUnit, SelectedAbility, targetUnit));
	}

	private ITurnOrderService turnOrderService;

	private IFactionService factionService;

	private Faction m_faction;
}
