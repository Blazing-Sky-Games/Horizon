using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
	public readonly Message<UnitLogic> UnitSelectedMessage = new Message<UnitLogic>();

	private void Start()
	{
		CoroutineManager.Main.StartCoroutine(WaitUiMain());
	}

	private void Update()
	{
		CoroutineManager.Main.UpdateCoroutines();
	}

	//initilize UI elements
	//TODO get rid of the need to do this by creating a static "Globals" class
	// that should be the only class with static members in the project, and it needs to be set up here
	private void Init()
	{
		//logic first
		Logic.Init();
		// the hotbar needs the turn order
		HotbarInterface.Init(Logic.TurnOrder);
		//the targeting infterface needs the selection channel and logic
		TargetingInterface.Init(UnitSelectedMessage, Logic);
		// combat display needs the turn order and the selected message
		CombatDisplay.Init(Logic.TurnOrder, UnitSelectedMessage);

		Logic.TurnOrder.CombatEncounterOverMessage.AddHandler(WaitHandleCombatOver);
	}

	IEnumerator WaitHandleCombatOver(bool win)
	{
		LogManager.Log(win ? "win" : "loss", LogDestination.Combat);
		SceneManager.LoadScene(win ? 1 : 2);
		yield break;
	}

	IEnumerator WaitHandleUnitSelected(UnitLogic arg)
	{
		HotbarInterface.SelectedUnit = arg;
		yield break;
	}

	IEnumerator WaitHandleAbilitySelected(UnitAbilityLogic arg)
	{
		//yuk TODO clean up this logic
		if(HotbarInterface.SelectedUnit == Logic.TurnOrder.ActiveUnit && Logic.TurnOrder.ActiveUnit.data.Faction == Faction.Player)
		{
			//bring up unit targeting diolouge and wait for it to close
			yield return new Routine(TargetingInterface.WaitSelectTarget(Logic.TurnOrder.ActiveUnit, arg));
		}
	}

	private IEnumerator WaitUiMain()
	{
		Init();

		while(true)
		{
			//wait for an message we care about
			while((UnitSelectedMessage.Idle && 
			      HotbarInterface.UnitAbilitySelectedMessage.Idle && 
			      HotbarInterface.PassTurnMessageChannel.Idle) ||
				  Logic.TurnOrder.ActiveUnit.data.Faction != Faction.Player)
			{
				yield return 0;
			}
			
			//the pass turn button was pressed
			//TODO this should be moved to the hotbar ui
			if(HotbarInterface.PassTurnMessageChannel.MessagePending)
			{
				// the user clikced the pass turn button, so declare that the chose to pass the turn
				yield return new Routine(HotbarInterface.PassTurnMessageChannel.WaitHandleMessage(Logic.GetFactionLeader(Logic.TurnOrder.ActiveUnit.data.Faction).WaitPassTurn));
			}
			//a unit was selected hmm TODO should this be moved to hotbar ui
			else if(UnitSelectedMessage.MessagePending)
			{
				//update the hot bar
				yield return new Routine(UnitSelectedMessage.WaitHandleMessage(WaitHandleUnitSelected));
			} 
			// an ability was selected
			else if(HotbarInterface.UnitAbilitySelectedMessage.MessagePending)
			{
				yield return new Routine(HotbarInterface.UnitAbilitySelectedMessage.WaitHandleMessage(WaitHandleAbilitySelected));
			}
		}
	}
}
