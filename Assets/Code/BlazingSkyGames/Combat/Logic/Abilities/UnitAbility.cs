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

using System.Collections.Generic;
using System.Collections;
using System;

public class UnitAbility
{
	public readonly Message CriticalHit = new Message();

	private readonly List<CombatEffect> combatEffects;
	private readonly List<CombatEffect> criticalEffects;
	private readonly float critChanceBonus;

	public readonly string Name;

	public UnitAbility(UnitAbilityData Data)
	{
		combatEffects = Data.CombatEffects;
		criticalEffects = Data.CriticalEffects;
		critChanceBonus = Data.CritChanceBonus;
		Name = Data.DebugName;
	}

	public IEnumerator WaitUse(Unit caster, Unit target)
	{
		ServiceLocator.GetService<ILoggingService>().Log(caster.Name + " Used ability " + Name + " on " + target.Name );
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(CombatEffect effect in combatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		float criticalSuccessThreshold = (caster.GetCriticalAccuracy() / target.GetCriticalAvoidance()) * 0.2f; 

		Random rand = new Random();

		if(rand.NextDouble() <= criticalSuccessThreshold + critChanceBonus)
		{
			ServiceLocator.GetService<ILoggingService>().Log("critical hit!");
			yield return new Routine(CriticalHit.WaitSend());

			foreach(CombatEffect effect in criticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}