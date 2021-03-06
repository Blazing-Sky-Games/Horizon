﻿/*
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
using System.Linq;

public class Unit
{
	public readonly Message<Unit, UnitAbility, Unit> AbilityUsedMessage = new Message<Unit, UnitAbility, Unit>();
	public List<UnitAbility> Abilities = new List<UnitAbility>();

	public readonly Statistic Strength;
	public readonly Statistic Intelligence;
	public readonly Statistic Stability;
	public readonly Statistic Insight;
	public readonly Statistic Skill;
	public readonly Statistic Vitality;

	public readonly string Name;

	public readonly HitPoints Health;

	public readonly Faction Faction;

	public Unit(UnitData Data)
	{
		Abilities = Data.Abilities.Select(abilityData => new UnitAbility(abilityData)).ToList();

		Name = Data.DebugName;

		Strength = new Statistic(Data.Strength);
		Intelligence = new Statistic(Data.Intelligence);
		Stability = new Statistic(Data.Stability);
		Insight = new Statistic(Data.Insight);
		Skill = new Statistic(Data.Skill);
		Vitality = new Statistic(Data.Vitality);

		Faction = Data.Faction;

		Health = new HitPoints(Data.Health, Data.MaxHealth);
	}

	public int GetCombatPotency (EffectType effectType)
	{
		return effectType == EffectType.Physical ? Strength.Value : Intelligence.Value;
	}

	public int GetCombatResistance (EffectType effectType)
	{
		return effectType == EffectType.Physical ? Stability.Value : Insight.Value;
	}

	public int GetCriticalPotency (EffectType effectType)
	{
		return Skill.Value;
	}

	public int GetCriticalResistance (EffectType effectType)
	{
		return GetCombatResistance(effectType);
	}

	//based on skill, int, str,
	public float GetCriticalAccuracy ()
	{
		return Skill.Value + Intelligence.Value / 2 + Strength.Value / 4;
	}

	//based on vitality, stability, insight
	public float GetCriticalAvoidance ()
	{
		return Vitality.Value + Stability.Value / 2 + Insight.Value / 4;
	}

	public Statistic GetStatistic (UnitStatatistic stat)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				return Insight;
			case(UnitStatatistic.Intelligence):
				return Intelligence;
			case(UnitStatatistic.Skill):
				return Skill;
			case(UnitStatatistic.Stability):
				return Stability;
			case(UnitStatatistic.Strength):
				return Strength;
			case(UnitStatatistic.Vitality):
				return Vitality;
			default:
				return null;
		}
	}

	public bool ActionPrevented
	{
		get
		{
			return ActionPreventedPoll.AnyVotes;
		}
	}

	public readonly Poll ActionPreventedPoll = new Poll();
}