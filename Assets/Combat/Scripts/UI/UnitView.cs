using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

// displayes a unit in combat
public class UnitView : MonoBehaviour
{
	
	// where the name and health are displayed
	// must be a child of the button
	public Text DisplayText;
	//click on it to select the unit
	public Button UnitSelectButton;
	
	// By convention, Init must be called on UI elements to supply them with dependacies
	public void Init(Unit unit, MessageChannel<Unit> UnitSelectedMessageChannel, TurnOrder turnOrder)
	{
		//set backing fields
		m_unit = unit;
		m_unitSelectedMessageChannel = UnitSelectedMessageChannel;
		m_turnOrder = turnOrder;

		// initilization
		DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;
		UnitSelectButton.onClick.AddListener(OnClickUnitSelect);

		// start main
		CoroutineManager.Main.StartCoroutine(WaitUnitViewMain());
	}

	// when the user clicks on the unit, send a unit selected message
	private void OnClickUnitSelect()
	{
		CoroutineManager.Main.StartCoroutine(m_unitSelectedMessageChannel.WaitSend(m_unit));
	}

	IEnumerator WaitHandleHurt(int arg)
	{
		//write to combat log
		Debug.Log(m_unit.UnitName + " took " + arg + " points of damage");
		// update the health display
		DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;
		yield break;
	}

	IEnumerator WaitHandleUnitKilled(Unit arg)
	{
		if(arg == m_unit)
		{
			//dont show the unit
			//TODO fix bug related to this
			UnitSelectButton.gameObject.SetActive(false);
			//write to combat log
			Debug.Log(m_unit.UnitName + " died");
		}

		yield break;
	}

	IEnumerator WaitHandleAbilityUsed(AbilityUsedMessageContent arg)
	{
		Debug.Log(arg.Caster.UnitName + " used " + arg.Ability.AbilityName + " on " + arg.Target.UnitName);
		yield break;
	}

	IEnumerator WaitHandleStatusChange(UnitStatus arg)
	{
		if(m_unit.GetStatus(arg))
		{
			Debug.Log(m_unit.UnitName + " was " + arg.ToString());
		}
		else
		{
			Debug.Log("status \"" + arg.ToString() + "\" ended for " + m_unit.UnitName);
		}

		yield break;
	}

	private IEnumerator WaitUnitViewMain()
	{
		// every frame while this game object is active
		while(true)
		{
			//wait for a message we care about
			while(m_unit.HurtMessage.Idle &&
			      m_unit.AbilityUsedMessage.Idle &&
			      m_turnOrder.UnitKilledMessage.Idle &&
			      m_unit.StatusChangedMessage.Idle)
			{
				yield return 0;
			}

			//the unit was hurt
			if(m_unit.HurtMessage.MessagePending)
			{
				yield return m_unit.HurtMessage.HandleMessage(WaitHandleHurt);
			}
			else if(m_unit.StatusChangedMessage.MessagePending)
			{
				yield return m_unit.StatusChangedMessage.HandleMessage(WaitHandleStatusChange);
			}
			// the unit used an ability
			else if(m_unit.AbilityUsedMessage.MessagePending)
			{
				yield return m_unit.AbilityUsedMessage.HandleMessage(WaitHandleAbilityUsed);
			}
			// the unit was killed
			else if(m_turnOrder.UnitKilledMessage.MessagePending)
			{
				yield return m_turnOrder.UnitKilledMessage.HandleMessage(WaitHandleUnitKilled);
			}
		}
	}

	// the unit this view is displaying
	private Unit m_unit;
	// send a message to this channel when this unit is selected
	private MessageChannel<Unit> m_unitSelectedMessageChannel;
	// use the turn order to check if a unit dies
	// TODO fix it so we dont need this here
	private TurnOrder m_turnOrder;
}
