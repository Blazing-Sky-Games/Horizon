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

	public IEnumerator WaitForSecondsMessage(float seconds, string message)
	{
		yield return 0;
		float elapsed = 0;
		while (elapsed < seconds)
		{
			Debug.Log(message);
			yield return 0;
			elapsed += Time.deltaTime;
		}
	}

	private void Start ()
	{
		StartCoroutine (UiMain ());
	}

	//initilize UI elements
	private void Init ()
	{
		Logic.Init ();
		HotbarInterface.Init (Logic.TurnOrder);
		TargetingInterface.Init (UnitSelectedMessage, Logic);
		CombatDisplay.Init (Logic.TurnOrder, UnitSelectedMessage);
	}

	private IEnumerator UiMain ()
	{
		Init ();

		bool EncounterOver = false;
		bool win = false;

		Logic.TurnOrder.CombatEncounterOverMessage.BeginProccesMessage();

		while (!EncounterOver)
		{
			//wait for an message we care about
			while (UnitSelectedMessage.Idle && 
			       HotbarInterface.UnitAbilitySelectedMessage.Idle && 
			       HotbarInterface.PassTurnMessage.Idle &&
			       Logic.TurnOrder.CombatEncounterOverMessage.Idle)
			{
				yield return 0;
			}

			if (Logic.TurnOrder.CombatEncounterOverMessage.State == MessageChannelState.MessagePending)
			{
				EncounterOver = true;
				win = Logic.TurnOrder.CombatEncounterOverMessage.Content;
			}

			if(Logic.TurnOrder.ActiveUnit.Faction != Faction.Player)
			{
				while (!(UnitSelectedMessage.Idle && 
				       	 HotbarInterface.UnitAbilitySelectedMessage.Idle && 
				       	 HotbarInterface.PassTurnMessage.Idle))
				{
					yield return 0;
				}

				continue;
			}
			
			//the pass turn button was pressed
			if (HotbarInterface.PassTurnMessage.State == MessageChannelState.MessagePending)
			{
				Logic.GetFactionLeader (Logic.TurnOrder.ActiveUnit.Faction).PassTurn ();

				yield return Logic.GetFactionLeader (Logic.TurnOrder.ActiveUnit.Faction).ActionDecidedMessage.WaitTillMessageProcessed ();
			}
			//a unit was selected
			else if (UnitSelectedMessage.State == MessageChannelState.MessagePending)
			{
				HotbarInterface.SelectedUnit = UnitSelectedMessage.Content;
				yield return UnitSelectedMessage.WaitTillMessageProcessed ();
			} 
			// an ability was selected
			else if (HotbarInterface.UnitAbilitySelectedMessage.State == MessageChannelState.MessagePending)
			{
				if (HotbarInterface.SelectedUnit == Logic.TurnOrder.ActiveUnit && Logic.TurnOrder.ActiveUnit.Faction == Faction.Player)
				{
					//bring up unit targeting diolouge and wait for it to close
					yield return TargetingInterface.WaitSelectTarget (Logic.TurnOrder.ActiveUnit, HotbarInterface.UnitAbilitySelectedMessage.Content);
				}

				yield return HotbarInterface.UnitAbilitySelectedMessage.WaitTillMessageProcessed ();
			}
		}

		Logic.TurnOrder.CombatEncounterOverMessage.EndProccesMessage();
		yield return Logic.TurnOrder.CombatEncounterOverMessage.WaitTillMessageProcessed ();

		yield return StartCoroutine (WaitForSecondsMessage (Random.value + 0.5f, win ? "you win" : "you lose"));
		Application.LoadLevel (win ? 1 : 2);
	}

	private Unit m_selectedUnit;
}
