using System;

public interface ICombatScenarioService : IService
{
	CombatScenario CurrentScenario{ get; }
}


