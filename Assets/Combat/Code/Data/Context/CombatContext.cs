using System;
using System.Collections;

public class CombatContext : MainContextBase
{
	private CombatLogic combatLogic;

	protected override Coroutine Load ()
	{
		base.Load();
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();

		ServiceLocator.GetService<ITurnOrderService>().LoadService();
		ServiceLocator.GetService<IFactionService>().LoadService();

		combatLogic = new CombatLogic(ServiceLocator.GetService<ICombatScenarioService>().CurrentScenario);

		return null;
	}

	public override void Unload ()
	{
		ServiceLocator.GetService<ITurnOrderService>().UnloadService();
		ServiceLocator.GetService<IFactionService>().UnloadService();

		ServiceLocator.RemoveService<ITurnOrderService>();
		ServiceLocator.RemoveService<IFactionService>();
	}
}