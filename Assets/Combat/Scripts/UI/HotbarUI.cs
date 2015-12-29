using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
	//ui elements supplyed in editor
	public Text StatsDisplay;
	public Button PassTurnBtn;
	public UnitAbilityButton UnitAbilityButtonPrefab;
	public Vector2 AbilityButtonOffset; // controls how the buttons are layed out
	public Vector2 AbilityButtonStride; // controls how the buttons are layed out
	
	// the pass turn button was clicked
	public readonly MessageChannel PassTurnMessageChannel = new MessageChannel ();
	// an ability was selected
	public readonly MessageChannel<UnitAbility> UnitAbilitySelectedMessage = new MessageChannel<UnitAbility> ();

	// by convention init is called on ui elements to provide dependaceis
	public void Init(TurnOrder turnOrder)
	{
		// set backing fields
		m_turnOrder = turnOrder;
		SelectedUnit = turnOrder.ActiveUnit;

		//init
		PassTurnBtn.onClick.AddListener (OnClickPassTurn);

		//start main
		CoroutineManager.Main.StartCoroutine (WaitHotbarMain());
	}

	IEnumerator WaitHandleTurnOrderAdvance ()
	{
		//write to combat log
		Debug.Log("advance turn order");
		SelectedUnit = m_turnOrder.ActiveUnit;
		yield break;
	}

	IEnumerator WaitHotbarMain ()
	{
		while (true)
		{
			//wait for a message we care about
			while(m_turnOrder.AdvanceTurnOrderMessage.Idle)
			{
				yield return 0;
			}

			//turn order advanced
			yield return m_turnOrder.AdvanceTurnOrderMessage.WaitHandleMessage(WaitHandleTurnOrderAdvance);
		}
	}

	// the unit displayed in the hot bar
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

	// send pass turn message when pass turn button clicked
	private void OnClickPassTurn ()
	{
		CoroutineManager.Main.StartCoroutine(PassTurnMessageChannel.WaitSend());
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
		StatsDisplay.text = newStatDisplayString;


		// destory the ability buttons
		// TODO uh oh, we are destroying views
		foreach (UnitAbilityButton button in m_abilityButtons)
		{
			Destroy(button.gameObject);
		}
		m_abilityButtons.Clear ();

		// re create the ability Buttons
		for (int i = 0; i < newUnit.abilities.Count; i++)
		{
			m_abilityButtons.Add (instatiateAbilityButton (newUnit.abilities[i]));
		}

		// set the abilities on the ability buttons
		// and put them in the right place
		for (int i = 0; i < m_abilityButtons.Count; i++)
		{
			m_abilityButtons [i].ButtonTrasform.localPosition = new Vector3 (AbilityButtonOffset.x + AbilityButtonStride.x * i, AbilityButtonOffset.y + AbilityButtonStride.y * i, 0);
		}
	}

	// creat a new ability button based on prefab
	private UnitAbilityButton instatiateAbilityButton (UnitAbility ability)
	{
		//instantiate the prefab
		UnitAbilityButton newbutton = Instantiate (UnitAbilityButtonPrefab);
		newbutton.Init (ability, UnitAbilitySelectedMessage);

		//make sure the button is scalled correctly
		newbutton.transform.SetParent (transform, false);
		newbutton.transform.localScale = new Vector3 (1, 1, 1);

		return newbutton;
	}

	// the unit this view is displaying
	private Unit m_showUnit;
	// use this to update the selected unit when the turn order advances
	private TurnOrder m_turnOrder;
	// where we store the ability buttons
	private List<UnitAbilityButton> m_abilityButtons = new List<UnitAbilityButton> ();
}
