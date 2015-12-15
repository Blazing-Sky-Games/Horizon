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

		StartCoroutine (CombatMain ());
	}

	private IEnumerator CombatMain ()
	{
		bool EncounterOver = false;

		while (!EncounterOver)
		{
			Actor FactionLeader = GetFactionLeader (m_turnOrder.ActiveUnit.Faction);

			FactionLeader.ResetCanTakeAction();

			while (FactionLeader.CanTakeAction)
			{
				//tell the actor to decide what to do
				FactionLeader.DecideAction();
				//wait for the current actor to decide an action
				yield return FactionLeader.ActionDecidedMessage.WaitForMessage();
				//perform that action and wait for it to finish
				m_turnOrder.CombatEncounterOverMessage.BeginProccesMessage();// TODO whaaa.....
				FactionLeader.ActionDecidedMessage.BeginProccesMessage();
				yield return FactionLeader.ActionDecidedMessage.MessageContent.WaitPerformAction();
				FactionLeader.ActionDecidedMessage.EndProccesMessage();

				yield return FactionLeader.ActionDecidedMessage.WaitTillMessageProcessed();

				if(m_turnOrder.CombatEncounterOverMessage.State == MessageChannelState.MessagePending)
				{
					EncounterOver = true;
				}

				m_turnOrder.CombatEncounterOverMessage.EndProccesMessage();
			}

			//advance the turn order
			yield return m_turnOrder.WaitAdvanceTurnOrder ();
		}
	}
	
	private TurnOrder m_turnOrder;
	private Dictionary<Faction, Actor> factionLeaders;
}
