using System;
using System.Collections;

public class CombatContext : MainContextBase
{
	//private CombatLogic combatLogic;

	protected override IEnumerator Load ()
	{
		yield return new Routine(base.Load());
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();

		yield return new Routine(ServiceLocator.GetService<ITurnOrderService>().LoadService());
		yield return new Routine(ServiceLocator.GetService<IFactionService>().LoadService());
	}

	protected override IEnumerator Launch ()
	{
		yield return new Routine(base.Launch());
	}

	public override void Unload ()
	{
		ServiceLocator.GetService<ITurnOrderService>().UnloadService();
		ServiceLocator.GetService<IFactionService>().UnloadService();

		ServiceLocator.RemoveService<ITurnOrderService>();
		ServiceLocator.RemoveService<IFactionService>();
	}
}