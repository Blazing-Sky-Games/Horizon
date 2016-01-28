using System;

public class CombatContext : MainContextBase
{
	public override void Update ()
	{
		throw new NotImplementedException();
	}

	protected override void RegisterCoreServices ()
	{
		ServiceLocator.RegisterService<ITurnOrderService,TurnOrderService>();
		ServiceLocator.RegisterService<IFactionService, FactionService>();
	}

	protected override void InstatiateCoreServices ()
	{
		throw new NotImplementedException();
	}

	protected override System.Collections.IEnumerator Launch ()
	{
		throw new NotImplementedException();
	}

	protected override void RemoveServiceReferences ()
	{
		throw new NotImplementedException();
	}

	protected override void RemoveCoreServices ()
	{
		throw new NotImplementedException();
	}
}