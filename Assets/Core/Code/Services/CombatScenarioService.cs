using System;

public class CombatScenarioService : Service, ICombatScenarioService
{
	public override void LoadService ()
	{
		base.LoadService();
		ServiceLocator.GetService<IResourceService>().GetResourceIdsOfType<CombatScenarioDirectory>;
	}

	public CombatScenario CurrentScenario
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}
	public Message ScenarioWillChange
	{
		get
		{
			throw new NotImplementedException();
		}
	}
	public Message ScenarioWillChanged
	{
		get
		{
			throw new NotImplementedException();
		}
	}
	
}


