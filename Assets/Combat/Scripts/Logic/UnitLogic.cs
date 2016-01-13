using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitLogic : DataDrivenLogic<UnitLogicData>
{
	//this unit has been hurt
	public readonly Message<UnitLogic, UnitAbilityLogic, UnitLogic> AbilityUsedMessage = new Message<UnitLogic, UnitAbilityLogic, UnitLogic>();
	public readonly Message<UnitStatus> StatusChangedMessage = new Message<UnitStatus>();
	public List<UnitAbilityLogic> Abilities = new List<UnitAbilityLogic>();

	public readonly Statistic<int> Strength;
	public readonly Statistic<int> Intelligence;
	public readonly Statistic<int> Stability;
	public readonly Statistic<int> Insight;
	public readonly Statistic<int> Skill;
	public readonly Statistic<int> Vitality;

	public readonly HitPoints Health;

	public UnitLogic(UnitLogicData Data) 
		: base(Data)
	{
		Abilities = Data.Abilities.Select(abilityData => new UnitAbilityLogic(abilityData)).ToList();
		Strength = new Statistic<int>(Data.Strength);
		Intelligence = new Statistic<int>(Data.Intelligence);
		Stability = new Statistic<int>(Data.Stability);
		Insight = new Statistic<int>(Data.Insight);
		Skill = new Statistic<int>(Data.Skill);
		Vitality = new Statistic<int>(Data.Vitality);

		Health = new HitPoints(Data.Health, Data.MaxHealth);
	}

	public Faction Faction
	{
		get{ return Data.Faction; }
	}

	public int GetCombatPotency (EffectType effectType)
	{
		return effectType == EffectType.Physical ? Data.Strength : Data.Intelligence;
	}

	public int GetCombatResistance(EffectType effectType)
	{
		return effectType == EffectType.Physical ? Data.Stability : Data.Insight;
	}

	public int GetCriticalPotency (EffectType effectType)
	{
		return Data.Skill;
	}

	public int GetCriticalResistance (EffectType effectType)
	{
		return GetCombatResistance(effectType);
	}

	public float GetCriticalChance()
	{
		//TODO impliment this
		return 0;
	}

	public float GetCriticalAvoidanceChance()
	{
		//TODO impliment this
		return 0;
	}

	//TODO get rid of this
	public int GetStatistic(UnitStatatistic stat)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				return Data.Insight;
			case(UnitStatatistic.Intelligence):
				return Data.Intelligence;
			case(UnitStatatistic.Skill):
				return Data.Skill;
			case(UnitStatatistic.Stability):
				return Data.Stability;
			case(UnitStatatistic.Strength):
				return Data.Strength;
			case(UnitStatatistic.Vitality):
				return Data.Vitality;
			default:
				return 0;
		}
	}

	//TODO hmm...should this be WaitSetStatistic and send a message
	//TODO get rid of this
	public void SetStatistic(UnitStatatistic stat, int value)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				Data.Insight = value;
				break;
			case(UnitStatatistic.Intelligence):
				Data.Intelligence = value;
				break;
			case(UnitStatatistic.Skill):
				Data.Skill = value;
				break;
			case(UnitStatatistic.Stability):
				Data.Stability = value;
				break;
			case(UnitStatatistic.Strength):
				Data.Strength = value;
				break;
			case(UnitStatatistic.Vitality):
				Data.Vitality = value;
				break;
		}
	}

	public bool CanTakeAction
	{
		get
		{
			//hack to get stun to work
			return GetStatus (UnitStatus.Stunned) == false;
		}
	}

	public bool Dead
	{
		get
		{
			return Health.Current == 0;
		}
	}

	public IEnumerator WaitSetStatus(UnitStatus status, bool active)
	{
		m_status[status] = active;
		yield return new Routine(StatusChangedMessage.WaitSend(status));
	}

	public bool GetStatus(UnitStatus status)
	{
		return m_status.ContainsKey(status) && m_status[status];
	}

	private Dictionary<UnitStatus,bool> m_status = new Dictionary<UnitStatus, bool>();
}


