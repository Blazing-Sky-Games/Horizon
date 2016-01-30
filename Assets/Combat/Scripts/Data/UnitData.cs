using System.Collections.Generic;

public class UnitData : Data
{
	public Faction Faction;
	public int MaxHealth;
	public int Health;
	public List<UnitAbilityData> Abilities;
	public int Strength; // phys dmg /crit
	public int Intelligence; // tech dmg /crit
	public int Stability; // phys def/crit res
	public int Insight; // tech def/crit res
	public int Skill; // crit dmg
	public int Vitality; // - crit chance for defender. also HP
}
