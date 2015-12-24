using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DmgType
{
	Physical,
	Tech
}

public enum Faction
{
	Player,
	AI
}

public class Unit : ScriptableObject
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
	
	//this unit has been hurt
	public readonly MessageChannel<int> HurtMessage = new MessageChannel<int> ();
	public readonly MessageChannel<AbilityUsedMessageContent> AbilityUsedMessage = new MessageChannel<AbilityUsedMessageContent> ();

	public void SetTurnOrder(TurnOrder turnOrder)
	{
		m_turnOrder = turnOrder;
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

	public int CalcDamageAgainst (int BaseDmg, DmgType dmgType, Unit Defender, string condition, out bool crit)
	{
		Unit Attacker = this;

		crit = false;
		
		double maximum = 1;
		double minimum = 0.8;
		
		double AttackPower = Random.value * (maximum - minimum) + minimum;
		Debug.Log (AttackPower + "That is the Attackpower");
		double CombatComparison = (float)(dmgType == DmgType.Physical ? Attacker.Strength : Attacker.Intelligence) / (dmgType == DmgType.Physical ? Defender.Stability : Defender.Insight);
		//Debug.Log (CombatComparison);

		//dmg = (str or Int of attacker) / (stability or insight of defender) * basedmg  * (random number between 0.8 and 1);
		double calcdmg = CombatComparison * BaseDmg * AttackPower * .6 + 5;
		
		// crit chance = (str or Int of attacker) / (stability or insight of defender) / 2 * 0.4
		double critChance = (dmgType == DmgType.Physical ? (float)Attacker.Strength : (float)Attacker.Intelligence) / (float)Defender.Vitality / 2 * 0.4;
		double CritSuccess = Random.value;
		//Debug.Log (CritSuccess + " Critical chance? " + critChance);
		if (CritSuccess <= critChance)
		{
			double critMult = 1.0 + (double)Attacker.Skill /  (double)(dmgType == DmgType.Physical ? Defender.Stability : Defender.Insight) / 2.0;
			//Debug.Log ("CRITICAL!!!");
			crit = true;
			if (condition != "None") 
			{
				Debug.Log (Defender.UnitName + " is now " + condition + " At this power " + critMult);
			} 
			else 
			{
				// crit multiplier = 1 + (skill of attacker / vitality of defender / 2);

				Debug.Log ("CRITICAL!!!!!!!!!!!!!  At times " + critMult);
				calcdmg *= critMult;
			}
		}
		
		return (int)calcdmg;
	}

	public IEnumerator RespondToAttackRoutine (int dmg)
	{
		Health -= dmg;

		HurtMessage.SendMessage (dmg);
		yield return HurtMessage.WaitTillMessageProcessed ();

		if (Health <= 0)
		{
			m_turnOrder.UnitKilled(this);
			yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
		}
	}

	private TurnOrder m_turnOrder;
}
