using System;
using System.Linq;
using System.Collections;

public class CombatScenarioService : Service, ICombatScenarioService
{
	public override IEnumerator WaitLoadService ()
	{
		yield return new Routine(base.WaitLoadService());
		m_resourceService = ServiceLocator.GetService<IResourceService>();

		yield return new Routine(m_resourceService.CombatScenarioDirectoryResource.WaitLoad());

		combatScenarioDirectory = m_resourceService.CombatScenarioDirectoryResource.Asset;
		yield return new Routine(combatScenarioDirectory.FirstScenarioResource.WaitLoad());
		m_currentScenario = combatScenarioDirectory.FirstScenarioResource.Asset;
	}

	public CombatScenario CurrentScenario
	{
		get
		{
			return m_currentScenario;
		}
	}

	private CombatScenario m_currentScenario;
	private CombatScenarioDirectory combatScenarioDirectory;
	private IResourceService m_resourceService;
}


