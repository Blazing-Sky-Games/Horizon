using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatMainView : DataFromEditorView<CombatMainLogic,CombatLogicData,EmptyData>
{
	// *UI objects must be set up with Init
	public HotbarView HotbarInterface;
	public CombatAreaView CombatDisplay;
	public TargetingView TargetingPopup;

	protected override void SetUp ()
	{
		base.SetUp();
		turnOrder = ServiceUtility.GetServiceReference<TurnOrder>();
		factionService = ServiceUtility.GetServiceReference<FactionService>();
		combatLogicMessages = ServiceUtility.GetServiceReference<CombatLogicMessages>();
		combatViewMessages = ServiceUtility.GetServiceReference<CombatViewMessages>();
	}

	protected override void AttachInstanceHandlers ()
	{
		combatLogicMessages.Dereference().CombatEncounterOver.AddHandler(OnCombatOver);
		combatViewMessages.Dereference().UnitSelected.AddHandler(OnUnitSelected);
		HotbarInterface.UnitAbilitySelectedMessage.AddHandler(OnAbilitySelected);
		HotbarInterface.PassTurnMessageChannel.AddHandler(OnPassTurn);
	}

	protected override void DetachInstanceHandlers ()
	{
		combatLogicMessages.Dereference().CombatEncounterOver.RemoveHandler(OnCombatOver);
		combatViewMessages.Dereference().UnitSelected.RemoveHandler(OnUnitSelected);
		HotbarInterface.UnitAbilitySelectedMessage.RemoveHandler(OnAbilitySelected);
		HotbarInterface.PassTurnMessageChannel.RemoveHandler(OnPassTurn);
	}

	IEnumerator OnCombatOver(bool win)
	{
		//LogManager.Log(win ? "win" : "loss", LogDestination.Screen);
		yield break;
	}

	IEnumerator OnUnitSelected(UnitLogic arg)
	{
		HotbarInterface.SelectedUnit = arg;
		yield break;
	}

	IEnumerator OnAbilitySelected(UnitAbilityLogic arg)
	{
		//yuk TODO clean up this logic
		if(HotbarInterface.SelectedUnit == turnOrder.Dereference().ActiveUnit && turnOrder.Dereference().ActiveUnit.Faction == Faction.Player)
		{
			Routine<UnitLogic> TargetUnitRoutine = TargetingPopup.TargetUnitAsync();
			yield return TargetUnitRoutine;

			var faction = turnOrder.Dereference().ActiveUnit.Faction;
			var factionLeader = factionService.Dereference().GetFactionLeader(faction);

			if(TargetUnitRoutine.Result != null)
				yield return new Routine(factionLeader.WaitUseUnitAbility(HotbarInterface.SelectedUnit,arg,TargetUnitRoutine.Result));
		}
	}

	private IEnumerator OnPassTurn()
	{
		yield return new Routine(factionService.Dereference().GetFactionLeader(turnOrder.Dereference().ActiveUnit.Faction).WaitPassTurn());
	}

	WeakReference<TurnOrder> turnOrder;
	WeakReference<FactionService> factionService;
	WeakReference<CombatLogicMessages> combatLogicMessages;
	WeakReference<CombatViewMessages> combatViewMessages;
}
