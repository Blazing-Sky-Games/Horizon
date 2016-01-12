using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitLogic
{
	public UnitLogicData data;

	//this unit has been hurt
	public readonly Message<int> HurtMessage = new Message<int>();
	public readonly Message<UnitLogic, UnitAbilityLogic, UnitLogic> AbilityUsedMessage = new Message<UnitLogic, UnitAbilityLogic, UnitLogic>();
	public readonly Message<UnitStatus> StatusChangedMessage = new Message<UnitStatus>();
	public List<UnitAbilityLogic> abilities = new List<UnitAbilityLogic>();

	public UnitLogic(UnitLogicData data)
	{
		this.data = data;
		abilities = data.Abilities.Select(abilityData => new UnitAbilityLogic(abilityData)).ToList();
	}

	public int GetStatistic(UnitStatatistic stat)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				return data.Insight;
			case(UnitStatatistic.Intelligence):
				return data.Intelligence;
			case(UnitStatatistic.Skill):
				return data.Skill;
			case(UnitStatatistic.Stability):
				return data.Stability;
			case(UnitStatatistic.Strength):
				return data.Strength;
			case(UnitStatatistic.Vitality):
				return data.Vitality;
			default:
				return 0;
		}
	}

	//TODO hmm...should this be WaitSetStatistic and send a message
	public void SetStatistic(UnitStatatistic stat, int value)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				data.Insight = value;
				break;
			case(UnitStatatistic.Intelligence):
				data.Intelligence = value;
				break;
			case(UnitStatatistic.Skill):
				data.Skill = value;
				break;
			case(UnitStatatistic.Stability):
				data.Stability = value;
				break;
			case(UnitStatatistic.Strength):
				data.Strength = value;
				break;
			case(UnitStatatistic.Vitality):
				data.Vitality = value;
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
			return data.Health <= 0;
		}
	}

	//TODO fix it so this is not needed
	public void SetTurnOrder(TurnOrder turnOrder)
	{
		m_turnOrder = turnOrder;
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

	//TODO deal with this being called when the unit is dead
	public IEnumerator WaitTakeDamage(int dmg, bool isCritical)
	{
		data.Health -= dmg;

		yield return new Routine(HurtMessage.WaitSend(dmg));

		if(data.Health <= 0)
		{
			yield return new Routine(m_turnOrder.WaitKillUnit(this));
		}
	}

	private TurnOrder m_turnOrder;
	private Dictionary<UnitStatus,bool> m_status = new Dictionary<UnitStatus, bool>();
}


