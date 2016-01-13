using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DataCatagory("Combat/Logic")]
public class UnitAbilityLogicData : Data
{
	//suppled in editor
	//public string AbilityName; TODO
	public float CritChanceBonus = 1;
	public List<AbilityEffectLogicData> CombatEffects;
	public List<AbilityEffectLogicData> CriticalEffects;
}

