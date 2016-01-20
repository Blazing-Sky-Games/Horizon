using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatMainView : DataFromEditorView<CombatMainLogic,CombatLogicData,EmptyData>
{
	// *UI objects must be set up with Init
	public HotbarView HotbarInterface;
	public CombatAreaView CombatDisplay;

	protected override void AttachInstanceHandlers ()
	{
		Horizon.Combat.Logic.Globals.CombatEncounterOver.AddHandler(HandleCombatOver);
	}

	protected override void DetachInstanceHandlers ()
	{
		Horizon.Combat.Logic.Globals.CombatEncounterOver.RemoveHandler(HandleCombatOver);
	}

	protected override IEnumerator MainRoutine ()
	{
		while(true)
		{
			//wait for an message we care about
			while((Horizon.Combat.Views.Globals.UnitSelected.Idle && 
				HotbarInterface.UnitAbilitySelectedMessage.Idle && 
				HotbarInterface.PassTurnMessageChannel.Idle) ||
				Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction != Faction.Player)
			{
				yield return 0;
			}

			//the pass turn button was pressed
			//TODO this should be moved to the hotbar ui
			if(HotbarInterface.PassTurnMessageChannel.MessagePending)
			{
				// the user clikced the pass turn button, so declare that the chose to pass the turn
				yield return new Routine(HotbarInterface.PassTurnMessageChannel.WaitHandleMessage(Logic.GetFactionLeader(Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction).WaitPassTurn));
			}
			//a unit was selected hmm TODO should this be moved to hotbar ui
			else if(Horizon.Combat.Views.Globals.UnitSelected.MessagePending)
			{
				//update the hot bar
				yield return new Routine(Horizon.Combat.Views.Globals.UnitSelected.WaitHandleMessage(HandleUnitSelected));
			} 
			// an ability was selected
			else if(HotbarInterface.UnitAbilitySelectedMessage.MessagePending)
			{
				yield return new Routine(HotbarInterface.UnitAbilitySelectedMessage.WaitHandleMessage(HandleAbilitySelected));
			}
		}
	}

	private void Update()
	{
		//TODO move this to core scene
		Horizon.Core.Logic.Globals.Coroutines.UpdateCoroutines();
	}

	IEnumerator HandleCombatOver(bool win)
	{
		//LogManager.Log(win ? "win" : "loss", LogDestination.Screen);
		SceneManager.LoadScene(win ? 1 : 2);
		yield break;
	}

	IEnumerator HandleUnitSelected(UnitLogic arg)
	{
		HotbarInterface.SelectedUnit = arg;
		yield break;
	}

	IEnumerator HandleAbilitySelected(UnitAbilityLogic arg)
	{
		//yuk TODO clean up this logic
		if(HotbarInterface.SelectedUnit == Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit && Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction == Faction.Player)
		{
			var TargetingInterface = gameObject.AddView<TargetingView,EmptyLogic,EmptyData,EmptyData>(EmptyLogic.Empty,Data.Empty);

			while(TargetingInterface.targ.Idle &&
				  TargetingInterface.CancelSelectTarget.Idle)
			{
				yield return 0;
			}

			if(TargetingInterface.TargetSelected.MessagePending)
			{
				UnitLogic activeunit = Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit;
				Actor leader = Horizon.Combat.Logic.Globals.GetFactionLeader(activeunit.Faction);

				//yield return new Routine(leader.WaitUseUnitAbility(activeunit,arg,))
			}
		}

		yield break;
	}
}
