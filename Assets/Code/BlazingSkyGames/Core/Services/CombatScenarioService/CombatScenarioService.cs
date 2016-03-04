/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
		yield return new Routine(combatScenarioDirectory.FirstScenario.WaitLoad());
		m_currentScenario = combatScenarioDirectory.FirstScenario.Asset;
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


