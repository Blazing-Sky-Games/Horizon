using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DataCatagory("Combat/Logic")]
public class UnitLogicData : Data
{
	//supplyed in editor
	public Faction Faction;
	public int MaxHealth;
	public int Health;
	//public string UnitName; TODO
	public List<UnitAbilityLogicData> Abilities;
	public int Strength; // phys dmg /crit
	public int Intelligence; // tech dmg /crit
	public int Stability; // phys def/crit res
	public int Insight; // tech def/crit res
	public int Skill; // crit dmg
	public int Vitality; // - crit chance for defender. also HP
}
