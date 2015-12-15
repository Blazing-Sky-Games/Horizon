using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
	//supplyed in editor
	public Text statsDisplay;
	public Button PassTurnBtn;
	public UnitAbilityButton UnitAbilityButtonPrefab;
	public Vector2 abilityButtonOffset;
	public Vector2 abilityButtonStep;
	
	// the pass turn button was clicked
	public readonly MessageChannel PassTurnMessage = new MessageChannel ();
	// an ability was selected
	public readonly MessageChannel<UnitAbility> UnitAbilitySelectedMessage = new MessageChannel<UnitAbility> ();

	public void Init(TurnOrder turnOrder)
	{
		m_turnOrder = turnOrder;
		SelectedUnit = turnOrder.ActiveUnit;
		PassTurnBtn.onClick.AddListener (OnClickPassTurn);
		StartCoroutine (HotbarMain());
	}

	IEnumerator HotbarMain ()
	{
		while (true)
		{
			yield return m_turnOrder.AdvanceTurnOrderMessage.WaitForMessage();

			m_turnOrder.AdvanceTurnOrderMessage.BeginProccesMessage();
				
				Debug.Log("advance turn order");

			m_turnOrder.AdvanceTurnOrderMessage.EndProccesMessage();

			SelectedUnit = m_turnOrder.ActiveUnit;
			yield return m_turnOrder.AdvanceTurnOrderMessage.WaitTillMessageProcessed();
		}
	}

	public Unit SelectedUnit
	{ 
		get
		{
			return m_showUnit;
		} 
		set
		{
			if (value != m_showUnit)
			{
				SetSelectedUnit (value);
				m_showUnit = value;
			}
		}
	}

	private void OnClickPassTurn ()
	{
		PassTurnMessage.SendMessage ();
	}

	private void SetSelectedUnit (Unit newUnit)
	{
		//update stat display
		string newStatDisplayString = "";
		newStatDisplayString += newUnit.UnitName + "\n";
		newStatDisplayString += "Stats \n";
		newStatDisplayString += "Owner : " + newUnit.Faction + "\n";
		newStatDisplayString += "HP : " + newUnit.Health + " / " + newUnit.MaxHealth + "\n";
		newStatDisplayString += "Strength : " + newUnit.Strength + "\n";
		newStatDisplayString += "Intelligence : " + newUnit.Intelligence + "\n";
		newStatDisplayString += "Stability : " + newUnit.Stability + "\n";
		newStatDisplayString += "Insight : " + newUnit.Insight + "\n";
		newStatDisplayString += "Skill : " + newUnit.Skill + "\n";
		newStatDisplayString += "Vitality : " + newUnit.Vitality + "\n";
		statsDisplay.text = newStatDisplayString;


		foreach (UnitAbilityButton button in abilityButtons)
		{
			Destroy(button.gameObject);
		}

		abilityButtons.Clear ();

		//if there are too few, add some
		for (int i = 0; i < newUnit.abilities.Count; i++)
		{
			abilityButtons.Add (instatiateAbilityButton (newUnit.abilities[i]));
		}

		//set the abilities on the ability buttons
		// and put them in the right place
		for (int i = 0; i < abilityButtons.Count; i++)
		{
			abilityButtons [i].rectTrasform.localPosition = new Vector3 (abilityButtonOffset.x + abilityButtonStep.x * i, abilityButtonOffset.y + abilityButtonStep.y * i, 0);
		}
	}

	private UnitAbilityButton instatiateAbilityButton (UnitAbility ability)
	{
		UnitAbilityButton newbutton = Instantiate (UnitAbilityButtonPrefab);
		newbutton.Init (ability, UnitAbilitySelectedMessage);
		newbutton.transform.SetParent (transform, false);
		newbutton.transform.localScale = new Vector3 (1, 1, 1);

		return newbutton;
	}

	private Unit m_showUnit;
	private TurnOrder m_turnOrder;
	private List<UnitAbilityButton> abilityButtons = new List<UnitAbilityButton> ();
}
