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

	public IEnumerator WaitForSecondsMessage(float seconds, string message)
	{
		yield return 0;
		float elapsed = 0;
		while (elapsed < seconds)
		{
			Debug.Log(message);
			yield return 0;
			elapsed += Time.deltaTime;
		}
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

					Coroutine displayDamage = StartCoroutine(WaitForSecondsMessage(Random.value / 2, m_unit.UnitName + " taking " + m_unit.HurtMessage.Content +  " points of damage"));
					Coroutine hurtAnim = StartCoroutine(WaitForSecondsMessage(Random.value / 2,"hurt animation"));

					yield return displayDamage;
					yield return hurtAnim;

				m_unit.HurtMessage.EndProccesMessage();

				DisplayText.text = m_unit.UnitName + " HP : " + m_unit.Health + " / " + m_unit.MaxHealth;
				yield return m_unit.HurtMessage.WaitTillMessageProcessed();
			}
			else if(m_unit.AbilityUsedMessage.State == MessageChannelState.MessagePending)
			{
				AbilityUsedMessageContext context = m_unit.AbilityUsedMessage.Content;

				m_unit.AbilityUsedMessage.BeginProccesMessage();

					//play attack animation. 
					yield return StartCoroutine(WaitForSecondsMessage(Random.value / 2,context.Caster.UnitName + " using " + context.Ability.AbilityName + " on " + context.Target.UnitName + " : wind up"));

					//when the animation reaches the "time of impact", trigger hurt animation, etc
					Coroutine endUseRotuine = StartCoroutine(context.Ability.EndUseAbilityRoutine (context));
					//continue the attack animation
					Coroutine coolDownRoutine = StartCoroutine(WaitForSecondsMessage(Random.value / 2,context.Caster.UnitName + " using " + context.Ability.AbilityName + " on " + context.Target.UnitName + " : cool down"));
					// critical hit display
					Coroutine critHit = StartCoroutine(WaitForSecondsMessage(Random.value / 2, "critical hit!"));

					//an ability is donw being used when its own animation is done and the resulting hurt/death animations are done
					yield return endUseRotuine;
					yield return coolDownRoutine;
					yield return critHit;

				m_unit.AbilityUsedMessage.EndProccesMessage();

				yield return m_unit.AbilityUsedMessage.WaitTillMessageProcessed();
			}
			else if(m_turnOrder.UnitKilledMessage.State == MessageChannelState.MessagePending && m_turnOrder.UnitKilledMessage.Content == m_unit)
			{
				m_turnOrder.UnitKilledMessage.BeginProccesMessage();

					yield return StartCoroutine(WaitForSecondsMessage(Random.value / 2,"DeathAnimation"));

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
