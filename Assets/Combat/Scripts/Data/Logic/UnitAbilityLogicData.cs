using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DataCatagory("Combat/Logic")]
public class UnitAbilityLogicData : Data
{
	//suppled in editor
	public string AbilityName;
	public float CritChanceMultiplyer = 1;
	public List<AbilityEffectLogicData> CombatEffects;
	public List<AbilityEffectLogicData> CriticalEffects;
}

