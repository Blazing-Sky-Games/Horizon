using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

class CombatLogicMessages : Service
{
	public Message<bool> CombatEncounterOver = new Message<bool>();
	public Message<UnitLogic> UnitKilled = new Message<UnitLogic>();
}

