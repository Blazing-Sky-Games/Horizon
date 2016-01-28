using System;
using System.Collections;

public class CombatContext : MainContextBase
{
	public override IEnumerator WaitLoad ()
	{
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();

		ServiceLocator.GetService<ITurnOrderService>().LoadService();
		ServiceLocator.GetService<IFactionService>().LoadService();

		yield break;
	}

	public override void Unload ()
	{
		ServiceLocator.GetService<ITurnOrderService>().UnloadService();
		ServiceLocator.GetService<IFactionService>().UnloadService();

		ServiceLocator.RemoveService<ITurnOrderService>();
		ServiceLocator.RemoveService<IFactionService>();
	}
}