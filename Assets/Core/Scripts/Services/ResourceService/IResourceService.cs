using System;
using System.Collections.Generic;
using System.Collections;

public class CombatScenarioDirectoryResourceReference : ResourceReference<CombatScenarioDirectory>{}

public interface IResourceService : IService
{
	CombatScenarioDirectoryResourceReference CombatScenarioDirectoryResource { get; }
}



