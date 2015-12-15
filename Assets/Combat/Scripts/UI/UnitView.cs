using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// displayes a unit in combat
public class UnitView : MonoBehaviour {
	
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
		UnitSelectButton.onClick.AddListener (OnClickUnitSelect);

		// start main
		StartCoroutine (UnitViewMain ());
	}

	// when the user clicks on the unit, send a unit selected message
	private void OnClickUnitSelect()
	{
		m_unitSelectedMessageChannel.SendMessage (m_unit); // TODO wait for m_unitSelectedMessageChannel
	}

	private IEnumerator UnitViewMain()
	{
		// every frame while this game object is active
		while (true)
		{
			//wait for a message we care about
			while(m_unit.HurtMessage.Idle &&
			      m_unit.AbilityUsedMessage.Idle &&
			      (m_turnOrder.UnitKilledMessage.Idle || m_turnOrder.UnitKilledMessage.MessageContent != m_unit))
			{
				yield return 0;
			}

			//the unit was hurt
			if(m_unit.HurtMessage.MessagePending)
			{
				m_unit.HurtMessage.BeginProccesMessage();

					//write to combat log
					Debug.Log(m_unit.UnitName + " took " + m_unit.HurtMessage.MessageContent +  " points of damage");
					// update the health display
					DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;

				m_unit.HurtMessage.EndProccesMessage();

				yield return m_unit.HurtMessage.WaitTillMessageProcessed();
			}
			// the unit used an ability
			else if(m_unit.AbilityUsedMessage.MessagePending)
			{
				m_unit.AbilityUsedMessage.BeginProccesMessage();

					AbilityUsedMessageContent content = m_unit.AbilityUsedMessage.MessageContent;
					//write to combat log
					Debug.Log(content.Caster.UnitName + " used " + content.Ability.AbilityName + " on " + content.Target.UnitName);
					
					// if there was a crit, write it to the combat log
					if(content.Crit)
					{
						Debug.Log("critical hit");
					}

					// affect the target
					yield return StartCoroutine(content.Ability.AffectTarget (content));

				m_unit.AbilityUsedMessage.EndProccesMessage();

				yield return m_unit.AbilityUsedMessage.WaitTillMessageProcessed();
			}
			// the unit was killed
			else if(m_turnOrder.UnitKilledMessage.MessagePending && m_turnOrder.UnitKilledMessage.MessageContent == m_unit)
			{
				m_turnOrder.UnitKilledMessage.BeginProccesMessage();

					//dont show the unit
					//TODO fix bug related to this
					UnitSelectButton.gameObject.SetActive(false);
					//write to combat log
					Debug.Log(m_unit.UnitName + " died");

				m_turnOrder.UnitKilledMessage.EndProccesMessage();

				yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
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
