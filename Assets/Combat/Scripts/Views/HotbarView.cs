using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HotbarView : View<UnitLogic,UnitLogicData,EmptyData>
{
	//ui elements supplyed in editor
	public Text StatsDisplay;
	public Button PassTurnBtn;
	public UnitAbilityButton UnitAbilityButtonPrefab;
	public Vector2 AbilityButtonOffset; // controls how the buttons are layed out
	public Vector2 AbilityButtonStride; // controls how the buttons are layed out
	// the pass turn button was clicked
	public readonly Message PassTurnMessageChannel = new Message();
	// an ability was selected
	public readonly Message<UnitAbilityLogic> UnitAbilitySelectedMessage = new Message<UnitAbilityLogic>();

	protected override void Awake ()
	{
		base.Awake();
		InjectLogic(Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit, false);
	}

	protected override void AttachInstanceHandlers ()
	{
		base.AttachInstanceHandlers();
		PassTurnBtn.onClick.AddListener(OnClickPassTurn);
	}

	protected override IEnumerator MainRoutine ()
	{
		while(true)
		{
			//wait for a message we care about
			while(Horizon.Combat.Logic.Globals.turnOrder.AdvanceTurnOrderMessage.Idle)
			{
				yield return 0;
			}

			//turn order advanced
			yield return new Routine(Horizon.Combat.Logic.Globals.turnOrder.AdvanceTurnOrderMessage.WaitHandleMessage(WaitHandleTurnOrderAdvance));
		}
	}

	IEnumerator WaitHandleTurnOrderAdvance()
	{
		//write to combat log
		//LogManager.Log("advance turn order", LogDestination.Screen); TODO
		SelectedUnit = Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit;
		yield break;
	}

	// the unit displayed in the hot bar
	public UnitLogic SelectedUnit
	{ 
		get
		{
			return Logic;
		} 
		set
		{
			SetLogic(value);
		}
	}

	// send pass turn message when pass turn button clicked
	private void OnClickPassTurn()
	{
		Horizon.Core.Logic.Globals.Coroutines.StartCoroutine(PassTurnMessageChannel.WaitSend());
	}

	//TODO bind to messages on stats
	private void SetSelectedUnit(UnitLogic newUnit)
	{
		//update stat display
		string newStatDisplayString = "";
		//newStatDisplayString += newUnit.data.UnitName + "\n"; TODO get this from view information
		newStatDisplayString += "Stats \n";
		newStatDisplayString += "Owner : " + newUnit.Faction + "\n";
		newStatDisplayString += "HP : " + newUnit.Health.Current + " / " + newUnit.Health.Max + "\n";
		newStatDisplayString += "Strength : " + newUnit.Strength.Value + "\n";
		newStatDisplayString += "Intelligence : " + newUnit.Intelligence.Value + "\n";
		newStatDisplayString += "Stability : " + newUnit.Stability.Value + "\n";
		newStatDisplayString += "Insight : " + newUnit.Insight.Value + "\n";
		newStatDisplayString += "Skill : " + newUnit.Skill.Value + "\n";
		newStatDisplayString += "Vitality : " + newUnit.Vitality.Value + "\n";
		StatsDisplay.text = newStatDisplayString;


		// destory the ability buttons
		// TODO uh oh, we are destroying views
		foreach(UnitAbilityButton button in m_abilityButtons)
		{
			Destroy(button.gameObject);
		}
		m_abilityButtons.Clear();

		// re create the ability Buttons
		for(int i = 0; i < newUnit.Abilities.Count; i++)
		{
			m_abilityButtons.Add(instatiateAbilityButton(newUnit.Abilities[i]));
		}

		// set the abilities on the ability buttons
		// and put them in the right place
		for(int i = 0; i < m_abilityButtons.Count; i++)
		{
			m_abilityButtons[i].ButtonTrasform.localPosition = new Vector3(AbilityButtonOffset.x + AbilityButtonStride.x * i, AbilityButtonOffset.y + AbilityButtonStride.y * i, 0);
		}
	}

	// creat a new ability button based on prefab
	private UnitAbilityButton instatiateAbilityButton(UnitAbilityLogic ability)
	{
		//instantiate the prefab
		UnitAbilityButton newbutton = Instantiate(UnitAbilityButtonPrefab);
		newbutton.Init(ability, UnitAbilitySelectedMessage);

		//make sure the button is scalled correctly
		newbutton.transform.SetParent(transform, false);
		newbutton.transform.localScale = new Vector3(1, 1, 1);

		return newbutton;
	}
		
	// where we store the ability buttons
	private List<UnitAbilityButton> m_abilityButtons = new List<UnitAbilityButton>();
}
