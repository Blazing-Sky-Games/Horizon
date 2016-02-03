using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class CombatScenarioResourceReference : ResourceReference<CombatScenario>{}

public class CombatScenarioDirectory : Data
{
	//right now this just says what scenario to load, but it will be fleshed out later
	public CombatScenarioResourceReference FirstScenario;
}



