using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitAbilityButton : MonoBehaviour
{
	public Text abilityName;
	public Button AbilityButton;
	
	public RectTransform rectTrasform
	{
		get
		{
			return AbilityButton.GetComponent<RectTransform> ();
		}
	}

	public void Init (UnitAbility ability, MessageChannel<UnitAbility> UnitAbilitySelectedMessage)
	{
		m_unitAbilitySelectedMessage = UnitAbilitySelectedMessage;
		m_ability = ability;
		abilityName.text = m_ability.AbilityName;

		AbilityButton.onClick.AddListener (OnClick);
	}

	public UnitAbility ability
	{
		get
		{
			return m_ability;
		}
	}

	void OnClick ()
	{
		m_unitAbilitySelectedMessage.SendMessage (m_ability);
	}

	private UnitAbility m_ability;
	private MessageChannel<UnitAbility> m_unitAbilitySelectedMessage;
}

