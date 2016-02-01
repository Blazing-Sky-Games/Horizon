using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ResourceService : Service, IResourceService
{
	public override IEnumerator WaitLoadService ()
	{
		m_combatScenarioDirectoryResource = ScriptableObject.CreateInstance<CombatScenarioDirectoryResourceReference>();

		m_combatScenarioDirectoryResource.ResourcePath = CombatScenarioPath;

		yield break;
	}

	public CombatScenarioDirectoryResourceReference CombatScenarioDirectoryResource
	{
		get
		{
			return m_combatScenarioDirectoryResource;
		}
	}
		
	private const string CombatScenarioPath = "CombatScenarioDirectory";
	private CombatScenarioDirectoryResourceReference m_combatScenarioDirectoryResource;
}




