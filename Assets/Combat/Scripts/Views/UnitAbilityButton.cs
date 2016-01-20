using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitAbilityButton : MonoBehaviour
{
	// ability name display. must be child of button
	public Text AbilityName;
	// click this to use the ability
	public Button AbilityButton;
	
	// use this to position the button
	public RectTransform ButtonTrasform
	{
		get
		{
			return AbilityButton.GetComponent<RectTransform>();
		}
	}

	// By convention, Init must be called on UI elements to supply them with dependacies
	public void Init(UnitAbilityLogic ability, Message<UnitAbilityLogic> unitAbilitySelectedMessageChannel)
	{
		//set backing fields
		m_unitAbilitySelectedMessageChannel = unitAbilitySelectedMessageChannel;
		m_ability = ability;

		//init
		//AbilityName.text = m_ability.AbilityName; TODO get this from view data
		AbilityButton.onClick.AddListener(OnClick);
	}

	// when the button is clicked, send a Ability selected message
	void OnClick()
	{
		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(WaitHandleOnClick());
	}

	IEnumerator WaitHandleOnClick()
	{
		// TODO disable all ability buttons
		AbilityButton.enabled = false;
		yield return new Routine(m_unitAbilitySelectedMessageChannel.WaitSend(m_ability));
		AbilityButton.enabled = true;
	}

	// the ability this view is displaying
	private UnitAbilityLogic m_ability;
	// send a message down this to select an ability
	private Message<UnitAbilityLogic> m_unitAbilitySelectedMessageChannel;
}

