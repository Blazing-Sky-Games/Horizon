using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// displayes a unit in combat
public class UnitView : View<UnitLogic,UnitLogicData,EmptyData>
{
	// where the name and health are displayed
	// must be a child of the button
	public Text DisplayText;
	//click on it to select the unit
	public Button UnitSelectButton;
	
	protected override IEnumerator MainRoutine ()
	{
		throw new System.NotImplementedException();
	}

	// By convention, Init must be called on UI elements to supply them with dependacies
	public void Init()
	{
		// initilization
		//DisplayText.text = m_unit.data.UnitName + " HP : " + m_unit.Health + " / " + m_unit.data.MaxHealth;
		DisplayText.text = " HP : " + Logic.Health.Current + " / " + Logic.Health.Max;
		UnitSelectButton.onClick.AddListener(OnClickUnitSelect);

		// start main
		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(WaitUnitViewMain());
	}

	// when the user clicks on the unit, send a unit selected message
	private void OnClickUnitSelect()
	{
		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(Horizon.Combat.Views.Globals.UnitSelected.WaitSend(Logic));
	}

	IEnumerator WaitHandleHurt()
	{
		//write to combat log
		//LogManager.Log(m_unit.data.UnitName + " took " + arg + " points of damage", LogDestination.Screen); todo name from viuew data
		// update the health display
		//DisplayText.text = m_unit.data.UnitName + " HP : " + m_unit.data.Health + " / " + m_unit.data.MaxHealth;
		DisplayText.text = " HP : " + Logic.Health.Current + " / " + Logic.Health.Max;
		yield break;
	}

	IEnumerator WaitHandleUnitKilled(UnitLogic unitKilled)
	{
		if(unitKilled == Logic)
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

	private IEnumerator WaitUnitViewMain()
	{
		// every frame while this game object is active
		while(true)
		{
			//wait for a message we care about
			while(Logic.Health.Hurt.Idle &&
			      Logic.AbilityUsedMessage.Idle &&
			      Horizon.Combat.Logic.Globals.UnitKilled.Idle &&
			      Logic.StatusChangedMessage.Idle)
			{
				yield return 0;
			}

			//the unit was hurt
			if(Logic.Health.Hurt.MessagePending)
			{
				yield return new Routine(Logic.Health.Hurt.WaitHandleMessage(WaitHandleHurt));
			}
			else if(Logic.StatusChangedMessage.MessagePending)
			{
				yield return new Routine(Logic.StatusChangedMessage.WaitHandleMessage(WaitHandleStatusChange));
			}
			// the unit used an ability
			else if(Logic.AbilityUsedMessage.MessagePending)
			{
				yield return new Routine(Logic.AbilityUsedMessage.WaitHandleMessage(WaitHandleAbilityUsed));
			}
			// the unit was killed
			else if(Horizon.Combat.Logic.Globals.UnitKilled.MessagePending)
			{
				yield return new Routine(Horizon.Combat.Logic.Globals.UnitKilled.WaitHandleMessage(WaitHandleUnitKilled));
			}
		}
	}
}
