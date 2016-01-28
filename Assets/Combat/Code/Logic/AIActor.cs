using System;
using System.Collections;
using System.Linq;

public class AIActor : Actor
{
	public AIActor(Faction faction)
	{
		m_faction = faction;
		turnOrderService = ServiceLocator.GetService<TurnOrderService>();
		factionService = ServiceLocator.GetService<FactionService>();
		unitService = ServiceLocator.GetService<IUnitService>();
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		Unit activeUnit = unitService.GetUnit(turnOrderService.ActiveUnitId);

		if(activeUnit.Faction.GetLeader() != this)
		{
			throw new InvalidOperationException("AI can only decide action when it is its turn");
		}
			
		UnitId targetUnitId = factionService.GetOpposingFaction(m_faction).GetUnits().RandomOrder().FirstOrDefault();
		if(targetUnitId == null)
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

		yield return new Routine(WaitUseUnitAbility(turnOrderService.ActiveUnitId, SelectedAbility, targetUnitId));
	}

	private ITurnOrderService turnOrderService;

	private IUnitService unitService;

	private IFactionService factionService;

	private Faction m_faction;
}
