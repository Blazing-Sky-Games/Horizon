using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CombatLogic : MonoBehaviour
{
	//supplyed in Editor
	public CombatScenario Scenario;

	public TurnOrder TurnOrder
	{
		get
		{
			return m_turnOrder;
		}
	}

	//get the Actor Associated with a faction
	public Actor GetFactionLeader (Faction faction)
	{
		return factionLeaders [faction];
	}

	public void Init ()
	{
		// deep copy so we are not editing the original version
		Scenario = Scenario.DeepCopy ();

		m_turnOrder = new TurnOrder (Scenario);

		//creat the actors that will be playing
		factionLeaders = new Dictionary<Faction, Actor> ();
		factionLeaders [Faction.Player] = new Actor ("player");
		factionLeaders [Faction.AI] = new AIActor("AI",this);

		CoroutineManager.Main.StartCoroutine (WaitCombatMain ());
	}

	private IEnumerator WaitCombatMain ()
	{
		bool EncounterOver = false;

		while (!EncounterOver)
		{
			Actor FactionLeader = GetFactionLeader (m_turnOrder.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while (FactionLeader.CanTakeAction)
			{
				//tell the actor to decide what to do
				CoroutineManager.Main.StartCoroutine(FactionLeader.WaitDecideAction());

				//wait for the current actor to decide an action
				while(FactionLeader.ActionDecidedMessage.Idle)
				{
					yield return 0;
				}
				//perform that action and wait for it to finish
				yield return FactionLeader.ActionDecidedMessage.HandleMessage(WaitHandleActionDecided);

				if(m_turnOrder.CombatEncounterOverMessage.MessagePending)
				{
					EncounterOver = true;
				}
			}

			//advance the turn order
			yield return m_turnOrder.WaitAdvance ();
			yield return TurnBasedEffectManager.WaitUpdateTurnBasedEffects();
		}
	}

	private IEnumerator WaitHandleActionDecided(IActorAction action)
	{
		yield return action.WaitPerform();
	}
	
	private TurnOrder m_turnOrder;
	private Dictionary<Faction, Actor> factionLeaders;
}
