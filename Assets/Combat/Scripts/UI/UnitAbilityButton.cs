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
	public void Init(UnitAbility ability, Message<UnitAbility> unitAbilitySelectedMessageChannel)
	{
		//set backing fields
		m_unitAbilitySelectedMessageChannel = unitAbilitySelectedMessageChannel;
		m_ability = ability;

		//init
		AbilityName.text = m_ability.AbilityName;
		AbilityButton.onClick.AddListener(OnClick);
	}

	// when the button is clicked, send a Ability selected message
	void OnClick()
	{
		CoroutineManager.Main.StartCoroutine(WaitHandleOnClick());
	}

	IEnumerator WaitHandleOnClick()
	{
		// TODO fix this weird thing. should you always have to wait for a message to be processed?
		// TODO lol i dont understand that old todo above this one
		AbilityButton.enabled = false;
		yield return new Routine(m_unitAbilitySelectedMessageChannel.WaitSend(m_ability));
		AbilityButton.enabled = true;
	}

	// the ability this view is displaying
	private UnitAbility m_ability;
	// send a message down this to select an ability
	private Message<UnitAbility> m_unitAbilitySelectedMessageChannel;
}

