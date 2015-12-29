using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitAbilityButton : MonoBehaviour
{
	// ability name display. must be child of button
	public Text abilityName;
	// click this to use the ability
	public Button AbilityButton;
	
	// use this to position the button
	public RectTransform ButtonTrasform
	{
		get
		{
			return AbilityButton.GetComponent<RectTransform> ();
		}
	}

	// By convention, Init must be called on UI elements to supply them with dependacies
	public void Init (UnitAbility ability, MessageChannel<UnitAbility> UnitAbilitySelectedMessageChannel)
	{
		//set backing fields
		m_unitAbilitySelectedMessageChannel = UnitAbilitySelectedMessageChannel;
		m_ability = ability;

		//init
		abilityName.text = m_ability.AbilityName;
		AbilityButton.onClick.AddListener (OnClick);
	}

	// when the button is clicked, send a Ability selected message
	void OnClick ()
	{
		CoroutineManager.Main.StartCoroutine (OnClickRoutine ());
	}

	IEnumerator OnClickRoutine()
	{
		// TODO fix this weird thing. should you always have to wait for a message to be processed?
		AbilityButton.enabled = false;
		yield return m_unitAbilitySelectedMessageChannel.Send(m_ability);
		AbilityButton.enabled = true;
	}

	// the ability this view is displaying
	private UnitAbility m_ability;
	// send a message down this to select an ability
	private MessageChannel<UnitAbility> m_unitAbilitySelectedMessageChannel;
}

