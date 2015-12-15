using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//displayes a unit in combat
public class UnitView : MonoBehaviour {
	
	public Text DisplayText;
	public Button UnitSelectButton;
	
	public void Init(Unit unit, MessageChannel<Unit> SelectedUnitMessage, TurnOrder turnOrder)
	{
		m_unit = unit;
		m_selectUnitMessage = SelectedUnitMessage;
		m_turnOrder = turnOrder;

		DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;

		UnitSelectButton.onClick.AddListener (OnClickUnitSelect);

		StartCoroutine (UnitViewMain ());
	}

	private void OnClickUnitSelect()
	{
		m_selectUnitMessage.SendMessage (m_unit);
	}

	private IEnumerator UnitViewMain()
	{
		while (true)
		{
			//wait for a message we care about
			while(m_unit.HurtMessage.Idle &&
			      m_unit.AbilityUsedMessage.Idle &&
			      (m_turnOrder.UnitKilledMessage.Idle || m_turnOrder.UnitKilledMessage.Content != m_unit))
			{
				yield return 0;
			}

			//the unit was hurt
			if(m_unit.HurtMessage.State == MessageChannelState.MessagePending)
			{
				m_unit.HurtMessage.BeginProccesMessage();

					Debug.Log(m_unit.UnitName + " took " + m_unit.HurtMessage.Content +  " points of damage");
					DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;

				m_unit.HurtMessage.EndProccesMessage();

				yield return m_unit.HurtMessage.WaitTillMessageProcessed();
			}
			else if(m_unit.AbilityUsedMessage.State == MessageChannelState.MessagePending)
			{
				m_unit.AbilityUsedMessage.BeginProccesMessage();

					AbilityUsedMessageContext context = m_unit.AbilityUsedMessage.Content;
					Debug.Log(context.Caster.UnitName + " used " + context.Ability.AbilityName + " on " + context.Target.UnitName);
					
					if(context.Crit)
					{
						Debug.Log("critical hit");
					}

					yield return StartCoroutine(context.Ability.EndUseAbilityRoutine (context));;

				m_unit.AbilityUsedMessage.EndProccesMessage();

				yield return m_unit.AbilityUsedMessage.WaitTillMessageProcessed();
			}
			else if(m_turnOrder.UnitKilledMessage.State == MessageChannelState.MessagePending && m_turnOrder.UnitKilledMessage.Content == m_unit)
			{
				m_turnOrder.UnitKilledMessage.BeginProccesMessage();

					UnitSelectButton.gameObject.SetActive(false);

				m_turnOrder.UnitKilledMessage.EndProccesMessage();

				yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
			}
		}
	}

	private Unit m_unit;
	private MessageChannel<Unit> m_selectUnitMessage;
	private TurnOrder m_turnOrder;
}
