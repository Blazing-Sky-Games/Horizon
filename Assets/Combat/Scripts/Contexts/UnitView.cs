using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// displayes a unit in combat
public class UnitView : View<UnitLogic,UnitLogicData,EmptyData>
{
	protected override void SetUp ()
	{
		base.SetUp();

		combatLogicMessages = ServiceUtility.GetServiceReference<CombatLogicMessages>();
		combatViewMessages = ServiceUtility.GetServiceReference<CombatViewMessages>();

		//displayText
		//UnitSelectButton
	}

	protected override void AttachInstanceHandlers ()
	{
		base.AttachInstanceHandlers();
		combatLogicMessages.Dereference().UnitKilled.AddHandler(WaitHandleUnitKilled);
		//UnitSelectButton.onClick.AddListener(OnClickUnitSelect);
	}

	protected override void AttachLogic ()
	{
		base.AttachLogic();
		Logic.Health.Hurt.AddHandler(WaitHandleHurt);
		Logic.AbilityUsedMessage.AddHandler(WaitHandleAbilityUsed);
		Logic.StatusChangedMessage.AddHandler(WaitHandleStatusChange);
	}

	// when the user clicks on the unit, send a unit selected message
	private void Select()
	{
		CoroutineUtility.StartCoroutine(combatViewMessages.Dereference().UnitSelected.WaitSend(Logic));
	}

	IEnumerator WaitHandleHurt()
	{
		//write to combat log
		//LogManager.Log(m_unit.data.UnitName + " took " + arg + " points of damage", LogDestination.Screen); todo name from viuew data
		// update the health display
		//DisplayText.text = m_unit.data.UnitName + " HP : " + m_unit.data.Health + " / " + m_unit.data.MaxHealth;
		//DisplayText.text = " HP : " + Logic.Health.Current + " / " + Logic.Health.Max;
		yield break;
	}

	IEnumerator WaitHandleUnitKilled(UnitLogic unitKilled)
	{
		if(unitKilled == Logic)
		{
			//dont show the unit
			//TODO fix bug related to this
			//UnitSelectButton.gameObject.SetActive(false);
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
		if(Logic.GetStatus(arg))
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

	// where the name and health are displayed
	// must be a child of the button
	//private Text DisplayText;
	//click on it to select the unit
	//private Button UnitSelectButton;

	WeakReference<CombatLogicMessages> combatLogicMessages;
	WeakReference<CombatViewMessages> combatViewMessages;
}
