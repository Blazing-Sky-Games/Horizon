using System;
using System.Collections;

public class CombatContext : MainContextBase
{
	private CombatLogic combatLogic;
	private CombatScenario scenario;

	protected override IEnumerator Load ()
	{
		yield return new Routine(base.Load());
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();
		ServiceLocator.RegisterService<IEnduringEffectService, EnduringEffectService>();
		ServiceLocator.RegisterService<IUnitService, UnitService>();

		yield return new Routine(ServiceLocator.GetService<ITurnOrderService>().LoadService());
		yield return new Routine(ServiceLocator.GetService<IFactionService>().LoadService());

		scenario = ServiceLocator.GetService<ICombatScenarioService>().CurrentScenario;
	}

	protected override IEnumerator Launch ()
	{
		yield return new Routine(base.Launch());

		combatLogic = new CombatLogic(scenario);
	}

	public override void Unload ()
	{
		ServiceLocator.GetService<ITurnOrderService>().UnloadService();
		ServiceLocator.GetService<IFactionService>().UnloadService();

		ServiceLocator.RemoveService<ITurnOrderService>();
		ServiceLocator.RemoveService<IFactionService>();
	}
}