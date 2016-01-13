using System.Collections;
using UnityEngine;
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
	public void Init(UnitLogic unit, Message<UnitLogic> unitSelectedMessageChannel, TurnOrder turnOrder)
	{
		//set backing fields
		m_unit = unit;
		m_unitSelectedMessageChannel = unitSelectedMessageChannel;
		m_turnOrder = turnOrder;

		// initilization
		//DisplayText.text = m_unit.data.UnitName + " HP : " + m_unit.Health + " / " + m_unit.data.MaxHealth;
		DisplayText.text = " HP : " + m_unit.Health.Current + " / " + m_unit.Health.Max;
		UnitSelectButton.onClick.AddListener(OnClickUnitSelect);

		// start main
		CoroutineManager.Main.StartCoroutine(WaitUnitViewMain());
	}

	// when the user clicks on the unit, send a unit selected message
	private void OnClickUnitSelect()
	{
		CoroutineManager.Main.StartCoroutine(m_unitSelectedMessageChannel.WaitSend(m_unit));
	}

	IEnumerator WaitHandleHurt()
	{
		//write to combat log
		//LogManager.Log(m_unit.data.UnitName + " took " + arg + " points of damage", LogDestination.Screen); TODO
		// update the health display
		//DisplayText.text = m_unit.data.UnitName + " HP : " + m_unit.data.Health + " / " + m_unit.data.MaxHealth;
		DisplayText.text = " HP : " + m_unit.Health.Current + " / " + m_unit.Health.Max;
		yield break;
	}

	IEnumerator WaitHandleUnitKilled(UnitLogic unitKilled)
	{
		if(unitKilled == m_unit)
		{
			//dont show the unit
			//TODO fix bug related to this
			UnitSelectButton.gameObject.SetActive(false);
			//write to combat log
			//LogManager.Log(m_unit.data.UnitName + " died", LogDestination.Screen); TODO
		}

		yield break;
	}

	IEnumerator WaitHandleAbilityUsed(UnitLogic Caster, UnitAbilityLogic Ability, UnitLogic Target)
	{
		//TODO
		//LogManager.Log(Caster.data.UnitName + " used " + Ability.data.AbilityName + " on " + Target.data.UnitName, LogDestination.Screen);
		yield break;
	}

	IEnumerator WaitHandleStatusChange(UnitStatus arg)
	{
		if(m_unit.GetStatus(arg))
		{
			//TODO
			//LogManager.Log(m_unit.data.UnitName + " was " + arg.ToString(), LogDestination.Screen);
		}
		else
		{
			//TODO
			//LogManager.Log("status \"" + arg.ToString() + "\" ended for " + m_unit.data.UnitName, LogDestination.Screen);
		}

		yield break;
	}

	private IEnumerator WaitUnitViewMain()
	{
		// every frame while this game object is active
		while(true)
		{
			//wait for a message we care about
			while(m_unit.Health.Hurt.Idle &&
			      m_unit.AbilityUsedMessage.Idle &&
			      m_turnOrder.UnitKilledMessage.Idle &&
			      m_unit.StatusChangedMessage.Idle)
			{
				yield return 0;
			}

			//the unit was hurt
			if(m_unit.Health.Hurt.MessagePending)
			{
				yield return new Routine(m_unit.Health.Hurt.WaitHandleMessage(WaitHandleHurt));
			}
			else if(m_unit.StatusChangedMessage.MessagePending)
			{
				yield return new Routine(m_unit.StatusChangedMessage.WaitHandleMessage(WaitHandleStatusChange));
			}
			// the unit used an ability
			else if(m_unit.AbilityUsedMessage.MessagePending)
			{
				yield return new Routine(m_unit.AbilityUsedMessage.WaitHandleMessage(WaitHandleAbilityUsed));
			}
			// the unit was killed
			else if(m_turnOrder.UnitKilledMessage.MessagePending)
			{
				yield return new Routine(m_turnOrder.UnitKilledMessage.WaitHandleMessage(WaitHandleUnitKilled));
			}
		}
	}

	// the unit this view is displaying
	private UnitLogic m_unit;
	// send a message to this channel when this unit is selected
	private Message<UnitLogic> m_unitSelectedMessageChannel;
	// use the turn order to check if a unit dies
	// TODO fix it so we dont need this here
	private TurnOrder m_turnOrder;
}
