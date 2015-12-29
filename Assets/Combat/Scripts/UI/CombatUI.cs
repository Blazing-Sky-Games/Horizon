using UnityEngine;
using System.Collections;

public class CombatUI : MonoBehaviour
{
	//supply in editor
	public CombatLogic Logic;

	// *UI objects must be set up with Init
	public HotbarUI HotbarInterface;
	public TargetingUI TargetingInterface;
	public CombatArea CombatDisplay;
	
	// notify that a unit has been selected
	// pass this to ui elements that need it
	public readonly MessageChannel<Unit> UnitSelectedMessage = new MessageChannel<Unit> ();

	private void Start ()
	{
		CoroutineManager.Main.StartCoroutine (UiMain ());
	}

	private void Update()
	{
		CoroutineManager.Main.UpdateCoroutines();
	}

	//initilize UI elements
	private void Init ()
	{
		//logic first
		Logic.Init ();
		// the hotbar needs the turn order
		HotbarInterface.Init (Logic.TurnOrder);
		//the targeting infterface needs the selection channel and logic
		TargetingInterface.Init (UnitSelectedMessage, Logic);
		// combat display needs the turn order and the selected message
		CombatDisplay.Init (Logic.TurnOrder, UnitSelectedMessage);
	}

	IEnumerator HandleCombatOver (bool arg)
	{
		win = arg;
		yield break;
	}

	bool win = false;

	IEnumerator HandleUnitSelected (Unit arg)
	{
		HotbarInterface.SelectedUnit = arg;
		yield break;
	}

	IEnumerator HandleAbilitySelected (UnitAbility arg)
	{
		//yuk TODO clean up this logic
		if (HotbarInterface.SelectedUnit == Logic.TurnOrder.ActiveUnit && Logic.TurnOrder.ActiveUnit.Faction == Faction.Player)
		{
			//bring up unit targeting diolouge and wait for it to close
			yield return TargetingInterface.SelectTarget (Logic.TurnOrder.ActiveUnit, arg);
		}
	}

	private IEnumerator UiMain ()
	{
		Init ();

		// book keeping vaiables to know when to exit
		// TODO clean this up
		bool EncounterOver = false;

		while (!EncounterOver)
		{
			//wait for an message we care about
			while (UnitSelectedMessage.Idle && 
			       HotbarInterface.UnitAbilitySelectedMessage.Idle && 
			       HotbarInterface.PassTurnMessageChannel.Idle &&
			       Logic.TurnOrder.CombatEncounterOverMessage.Idle)
			{
				yield return 0;
			}

			//the game is over
			if (Logic.TurnOrder.CombatEncounterOverMessage.MessagePending)
			{
				EncounterOver = true;
				yield return Logic.TurnOrder.CombatEncounterOverMessage.HandleMessage(HandleCombatOver);
			}

			// it is not the players turn, so we are not going to process any other messages
			// TODO fix it so we dont need this weird logic to handel the case where it is not the players turn
			if(Logic.TurnOrder.ActiveUnit.Faction != Faction.Player)
			{
				//wait for the messages to be proccessed
				while (!(UnitSelectedMessage.Idle && 
				       	 HotbarInterface.UnitAbilitySelectedMessage.Idle && 
				       	 HotbarInterface.PassTurnMessageChannel.Idle))
				{
					yield return 0;
				}

				// start checking for messages again
				continue;
			}
			
			//the pass turn button was pressed
			if (HotbarInterface.PassTurnMessageChannel.MessagePending)
			{
				// the user clikced the pass turn button, so declare that the chose to pass the turn
				yield return HotbarInterface.PassTurnMessageChannel.HandleMessage(Logic.GetFactionLeader (Logic.TurnOrder.ActiveUnit.Faction).PassTurn);
			}
			//a unit was selected hmm TODO should this be moved to hotbar ui
			else if (UnitSelectedMessage.MessagePending)
			{
				//update the hot bar
				yield return UnitSelectedMessage.HandleMessage(HandleUnitSelected);
			} 
			// an ability was selected
			else if (HotbarInterface.UnitAbilitySelectedMessage.MessagePending)
			{
				yield return HotbarInterface.UnitAbilitySelectedMessage.HandleMessage(HandleAbilitySelected);
			}
		}

		// load the correct level with the game over message
		Application.LoadLevel (win ? 1 : 2);
	}
}
