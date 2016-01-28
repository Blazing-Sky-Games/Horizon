using System;

public interface ICombatScenarioService : IService
{
	CombatScenario CurrentScenario{ get; set; }

	Message ScenarioWillChange { get; }

	Message ScenarioWillChanged { get; }
}


