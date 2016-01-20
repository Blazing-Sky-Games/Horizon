using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitLogic : ViewLogic<UnitLogicData>
{
	//this unit has been hurt
	public readonly Message<UnitLogic, UnitAbilityLogic, UnitLogic> AbilityUsedMessage = new Message<UnitLogic, UnitAbilityLogic, UnitLogic>();
	public readonly Message<UnitStatus> StatusChangedMessage = new Message<UnitStatus>();
	public List<UnitAbilityLogic> Abilities = new List<UnitAbilityLogic>();

	public readonly Statistic Strength;
	public readonly Statistic Intelligence;
	public readonly Statistic Stability;
	public readonly Statistic Insight;
	public readonly Statistic Skill;
	public readonly Statistic Vitality;

	public readonly HitPoints Health;

	public readonly Faction Faction;

	public UnitLogic(UnitLogicData Data) 
		: base(Data)
	{
		Abilities = Data.Abilities.Select(abilityData => new UnitAbilityLogic(abilityData)).ToList();

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

	public int GetCombatResistance(EffectType effectType)
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
	public float GetCriticalAccuracy()
	{
		return Skill.Value + Intelligence.Value / 2 + Strength.Value / 4;
	}

	// vitality, stability, insight
	public float GetCriticalAvoidance()
	{
		return Vitality.Value + Stability.Value / 2 + Insight.Value / 4;
	}

	//TODO CHANGE this to return a statistic instead of just an int
	public Statistic GetStatistic(UnitStatatistic stat)
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

	public bool CanTakeAction
	{
		get
		{
			//hack to get stun to work
			return GetStatus (UnitStatus.Stunned) == false;
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

	public override void Destroy ()
	{
		//nothing
	}

	private Dictionary<UnitStatus,bool> m_status = new Dictionary<UnitStatus, bool>();
}


