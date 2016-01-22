using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//bring up this ui to notify the user to target a Unit
public class TargetingView : View<EmptyLogic,EmptyData,EmptyData>
{
	protected override void SetUp ()
	{
		base.SetUp();

		combatViewMessages = ServiceUtility.GetServiceReference<CombatViewMessages>();

		//TargetingPrompt;
		//Cancel;
	}

	protected override void AttachInstanceHandlers ()
	{
		base.AttachInstanceHandlers();
		//Cancel.onClick.AddListener(OnCancleClick);
		combatViewMessages.Dereference().UnitSelected.AddAction(OnUnitSelected);
	}

	protected override void DetachInstanceHandlers ()
	{
		base.DetachInstanceHandlers();
		//Cancel.onClick.RemoveListener(OnCancleClick);
		combatViewMessages.Dereference().UnitSelected.RemoveAction(OnUnitSelected);
	}

	protected override void TearDown ()
	{
		base.TearDown();

		//TargetingPrompt;
		//Cancel;
	}

	public Routine<UnitLogic> TargetUnitAsync()
	{
		return new Routine<UnitLogic>(targetUnitRoutine());
	}

	IEnumerator targetUnitRoutine()
	{
		canceled = false;
		target = null;

		while(!canceled && target == null)
			yield return 0;

		yield return target;
	}

	void OnUnitSelected(UnitLogic unit)
	{
		target = unit;
	}

	void OnCancleClick()
	{
		canceled = true;
	}

	private bool canceled;
	private UnitLogic target;

	//private GameObject TargetingPrompt;
	//private Button Cancel;

	WeakReference<CombatViewMessages> combatViewMessages;
}

