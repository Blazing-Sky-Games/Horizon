using System;
using System.Linq;
using System.Collections;

public class CombatScenarioService : Service, ICombatScenarioService
{
	public override IEnumerator LoadService ()
	{
		yield return new Routine(base.LoadService());
		//m_resourceService = ServiceLocator.GetService<IResourceService>();

		//combatScenarioDirectory = m_resourceService.Resources.Where(res => res.ResourceType == typeof(CombatScenarioDirectory)).First().GetAsset() as CombatScenarioDirectory;
	}

	public CombatScenario CurrentScenario
	{
		get
		{
			//return combatScenarioDirectory.Scenario;
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
	public Message ScenarioChanged
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	//private CombatScenarioDirectory combatScenarioDirectory;
	//private IResourceService m_resourceService;
}


