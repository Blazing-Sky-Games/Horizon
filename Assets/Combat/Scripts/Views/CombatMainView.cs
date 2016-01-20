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
		Horizon.Combat.Views.Globals.UnitSelected.AddHandler(HandleUnitSelected);
		HotbarInterface.UnitAbilitySelectedMessage.AddHandler(HandleAbilitySelected);
		HotbarInterface.PassTurnMessageChannel.AddHandler(HandelPassTurn);
	}

	protected override void DetachInstanceHandlers ()
	{
		Horizon.Combat.Logic.Globals.CombatEncounterOver.RemoveHandler(HandleCombatOver);
		Horizon.Combat.Views.Globals.UnitSelected.RemoveHandler(HandleUnitSelected);
		HotbarInterface.UnitAbilitySelectedMessage.RemoveHandler(HandleAbilitySelected);
		HotbarInterface.PassTurnMessageChannel.RemoveHandler(HandelPassTurn);
	}

	IEnumerator HandleCombatOver(bool win)
	{
		//LogManager.Log(win ? "win" : "loss", LogDestination.Screen);
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

	private IEnumerator HandelPassTurn()
	{
		yield return new Routine(Horizon.Combat.Logic.Globals.GetFactionLeader(Horizon.Combat.Logic.Globals.turnOrder.ActiveUnit.Faction).WaitPassTurn());
	}
}
