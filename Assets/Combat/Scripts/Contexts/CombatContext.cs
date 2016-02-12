using System;
using System.Collections;

public class CombatContext : MainContextBase
{
	public AbilityContext ability;

	public Message<Unit> SelectUnitRequest;
	public Message<Unit> SelectedUnitChanged;

	//private CombatLogic combatLogic;
	private CombatScenario scenario;

	protected override IEnumerator WaitLoad ()
	{
		yield return new Routine(base.WaitLoad());
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();
		ServiceLocator.RegisterService<IEnduringEffectService, EnduringEffectService>();
		ServiceLocator.RegisterService<IUnitService, UnitService>();

		yield return new Routine(ServiceLocator.GetService<ITurnOrderService>().WaitLoadService());
		yield return new Routine(ServiceLocator.GetService<IFactionService>().WaitLoadService());

		scenario = ServiceLocator.GetService<ICombatScenarioService>().CurrentScenario;
		//combatLogic = new CombatLogic(scenario);


		Unit unit =	ServiceLocator.GetService<IUnitService>().CreateUnit(scenario.Units[0]);

		ability = new AbilityContext(unit.Abilities[0], unit);
	}

	protected override IEnumerator WaitLaunch ()
	{
		yield return new Routine(base.WaitLaunch());

		//combatLogic.Launch();
	}

	public override void Unload ()
	{
		ServiceLocator.GetService<ITurnOrderService>().UnloadService();
		ServiceLocator.GetService<IFactionService>().UnloadService();

		ServiceLocator.RemoveService<ITurnOrderService>();
		ServiceLocator.RemoveService<IFactionService>();
	}
}