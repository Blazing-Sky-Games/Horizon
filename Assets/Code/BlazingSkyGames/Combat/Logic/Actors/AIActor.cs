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
using System.Collections;
using System.Linq;

public class AIActor : Actor
{
	public AIActor(Faction faction, string Name) : base (Name)
	{
		m_faction = faction;
		turnOrderService = ServiceLocator.GetService<ITurnOrderService>();
		factionService = ServiceLocator.GetService<IFactionService>();
	}

	//pick a random ability and a random target and use that ability
	public override IEnumerator WaitDecideAction()
	{
		Unit activeUnit = turnOrderService.ActiveUnit;

		if(activeUnit.Faction.GetLeader() != this)
		{
			throw new InvalidOperationException("AI can only decide action when it is its turn");
		}
			
		Unit targetUnit = factionService.GetOpposingFaction(m_faction).GetUnits().RandomOrder().FirstOrDefault();
		if(targetUnit == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		UnitAbility SelectedAbility = activeUnit.Abilities.RandomOrder().FirstOrDefault();
		if(SelectedAbility == null)
		{
			yield return new Routine(WaitPassTurn());
			yield break;
		}

		yield return new Routine(WaitUseUnitAbility(turnOrderService.ActiveUnit, SelectedAbility, targetUnit));
	}

	private ITurnOrderService turnOrderService;

	private IFactionService factionService;

	private Faction m_faction;
}
