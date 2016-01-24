using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

class CombatViewMessages : Service
{
	public Message<UnitLogic> UnitSelected = new Message<UnitLogic>();
	public Message<UnitAbilityLogic> AbilitySelected = new Message<UnitAbilityLogic>();
}

