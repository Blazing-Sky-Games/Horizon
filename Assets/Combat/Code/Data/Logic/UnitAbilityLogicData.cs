using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DataCatagory("Combat/Logic")]
public class UnitAbilityLogicData : Data
{
	[UnityEngine.Tooltip("value from 0 to 1 that adds to crit liclyhood")]
	public float CritChanceBonus = 0;
	[UnityEngine.Tooltip("these effects always happen")]
	public List<CombatEffect> CombatEffects;
	[UnityEngine.Tooltip("these effects only happen on a succsesfull crit")]
	public List<CombatEffect> CriticalEffects;
}

