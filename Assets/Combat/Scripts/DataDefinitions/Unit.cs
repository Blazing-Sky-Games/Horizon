using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public enum EffectType
{
	Physical,
	Tech
}

public enum Faction
{
	Player,
	AI
}

public enum UnitStatus
{
	Poisoned,
	Stunned,
	Weakened
}

public class Unit : UnityEngine.ScriptableObject
{
	//supplyed in editor
	public Faction Faction;
	public int MaxHealth;
	public int Health;
	public string UnitName;
	public List<UnitAbility> abilities;
	public int Strength; // phys dmg /crit
	public int Intelligence; // tech dmg /crit
	public int Stability; // phys def/crit res
	public int Insight; // tech def/crit res
	public int Skill; // crit dmg
	public int Vitality; // - crit chance for defender. also HP

	public int GetStat(UnitStat stat)
	{
		switch(stat)
		{
		case(UnitStat.Insight):
			return Insight;
			//break;
		case(UnitStat.Intelligence):
			return Intelligence;
			//break;
		case(UnitStat.Skill):
			return Skill;
			//break;
		case(UnitStat.Stability):
			return Stability;
			//break;
		case(UnitStat.Strength):
			return Strength;
			//break;
		case(UnitStat.Vitality):
			return Vitality;
			//break;
		default:
			return 0;
		}
	}

	public void SetStat(UnitStat stat, int value)
	{
		switch(stat)
		{
		case(UnitStat.Insight):
			Insight = value;
			break;
		case(UnitStat.Intelligence):
			Intelligence = value;
			break;
		case(UnitStat.Skill):
			Skill = value;
			break;
		case(UnitStat.Stability):
			Stability = value;
			break;
		case(UnitStat.Strength):
			Strength = value;
			break;
		case(UnitStat.Vitality):
			Vitality = value;
			break;
		}
	}

	//this unit has been hurt
	public readonly MessageChannel<int> HurtMessage = new MessageChannel<int> ();
	public readonly MessageChannel<AbilityUsedMessageContent> AbilityUsedMessage = new MessageChannel<AbilityUsedMessageContent> ();

	public readonly MessageChannel<UnitStatus> StatusChangedMessage = new MessageChannel<UnitStatus>(); 

	public void SetTurnOrder(TurnOrder turnOrder)
	{
		m_turnOrder = turnOrder;
	}

	public IEnumerator SetStatus (UnitStatus status, bool active)
	{
		m_status[status] = active;
		yield return StatusChangedMessage.Send(status);
	}

	public bool GetStatus (UnitStatus status)
	{
		return m_status.ContainsKey(status) && m_status[status];
	}

	public Unit DeepCopy()
	{
		Unit newUnit = UnityEngine.Object.Instantiate<Unit>(this);

		List<UnitAbility> newAbilities = new List<UnitAbility> ();
		foreach (UnitAbility ability in abilities)
		{
			newAbilities.Add (ability.DeepCopy());
		}

		newUnit.abilities = newAbilities;

		return newUnit;
	}

	public IEnumerator TakeDamage(int dmg, bool IsCritical)
	{
		Health -= dmg;

		yield return HurtMessage.Send(dmg);

		if(Health <= 0)
		{
			yield return m_turnOrder.UnitKilled(this);
		}
	}

	private TurnOrder m_turnOrder;
	private Dictionary<UnitStatus,bool> m_status = new Dictionary<UnitStatus, bool>();
}
