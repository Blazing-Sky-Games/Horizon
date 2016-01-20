using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//bring up this ui to notify the user to target a Unit
public class TargetingView : View<EmptyLogic,EmptyData,EmptyData>
{
	public Message<UnitLogic> TargetSelected = new Message<UnitLogic>();

	protected override void SetUp ()
	{
		base.SetUp();

		//TargetingPrompt;
		//Cancel;
	}

	protected override void AttachInstanceHandlers ()
	{
		base.AttachInstanceHandlers();
		Cancel.onClick.AddListener(OnCancleClick);
		Horizon.Combat.Views.Globals.UnitSelected.AddHandler(HandelUnitSelected);
	}

	protected override void DetachInstanceHandlers ()
	{
		base.DetachInstanceHandlers();
		Cancel.onClick.RemoveListener(OnCancleClick);
		Horizon.Combat.Views.Globals.UnitSelected.RemoveHandler(HandelUnitSelected);
	}

	protected override void TearDown ()
	{
		base.TearDown();

		//TargetingPrompt;
		//Cancel;
	}

	IEnumerator HandelUnitSelected(UnitLogic unit)
	{
		yield return new Routine(TargetSelected.WaitSend(unit));
		Destroy(this);
	}
		
	void OnCancleClick()
	{
		Destroy(this);
	}

	private UnitLogic target;

	private GameObject TargetingPrompt;
	private Button Cancel;
}

