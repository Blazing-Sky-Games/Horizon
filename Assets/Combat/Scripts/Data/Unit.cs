using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Unit : UnityEngine.ScriptableObject
{
	//supplyed in editor
	public Faction Faction;
	public int MaxHealth;
	public int Health;
	public string UnitName;
	public List<UnitAbility> Abilities;
	public int Strength; // phys dmg /crit
	public int Intelligence; // tech dmg /crit
	public int Stability; // phys def/crit res
	public int Insight; // tech def/crit res
	public int Skill; // crit dmg
	public int Vitality; // - crit chance for defender. also HP

	//this unit has been hurt
	public readonly Message<int> HurtMessage = new Message<int>();
	public readonly Message<AbilityUsedMessageContent> AbilityUsedMessage = new Message<AbilityUsedMessageContent>();
	public readonly Message<UnitStatus> StatusChangedMessage = new Message<UnitStatus>();

	public int GetStatistic(UnitStatatistic stat)
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
				return 0;
		}
	}

	//TODO hmm...should this be WaitSetStatistic and send a message
	public void SetStatistic(UnitStatatistic stat, int value)
	{
		switch(stat)
		{
			case(UnitStatatistic.Insight):
				Insight = value;
				break;
			case(UnitStatatistic.Intelligence):
				Intelligence = value;
				break;
			case(UnitStatatistic.Skill):
				Skill = value;
				break;
			case(UnitStatatistic.Stability):
				Stability = value;
				break;
			case(UnitStatatistic.Strength):
				Strength = value;
				break;
			case(UnitStatatistic.Vitality):
				Vitality = value;
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
			return Health <= 0;
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

	public Unit DeepCopy()
	{
		Unit newUnit = UnityEngine.Object.Instantiate<Unit>(this);

		List<UnitAbility> newAbilities = new List<UnitAbility>();
		foreach(UnitAbility ability in Abilities)
		{
			newAbilities.Add(ability.DeepCopy());
		}

		newUnit.Abilities = newAbilities;

		return newUnit;
	}

	//TODO deal with this being called when the unit is dead
	public IEnumerator WaitTakeDamage(int dmg, bool isCritical)
	{
		Health -= dmg;

		yield return new Routine(HurtMessage.WaitSend(dmg));

		if(Health <= 0)
		{
			yield return new Routine(m_turnOrder.WaitKillUnit(this));
		}
	}

	private TurnOrder m_turnOrder;
	private Dictionary<UnitStatus,bool> m_status = new Dictionary<UnitStatus, bool>();
}
