using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//bring up this ui to notify the user to target a Unit
public class TargetingUI : MonoBehaviour
{
	//supplyed in editor
	//root game object of the ui elements, but NOT the game object this script is attached to
	public GameObject TargetingPrompt;
	public Button Cancel;
	// the action selected as a result of briging this UI up
	public IActorAction Result;

	// By convention, Init must be called on UI elements to supply them with dependacies
	public void Init(Message<Unit> selectUnitMessage, CombatLogic logic)
	{
		m_selectUnitMessage = selectUnitMessage;
		m_logic = logic;

		Cancel.onClick.AddListener(OnCancleClick);
	}

	// send cancel message if the user backs out of selecting a unit
	void OnCancleClick()
	{
		m_canceled = true;
	}
	
	IEnumerator WaitHandleUnitSelected(Unit arg)
	{
		// the user has selected a target, so declare that they have used an ability
		yield return new Routine(m_logic.GetFactionLeader(m_caster.Faction).WaitUseUnitAbility(m_caster, m_ability, arg));
	}

	// call this to bring up the ui and select a target for an ability
	public IEnumerator WaitSelectTarget(Unit caster, UnitAbility ability)
	{
		m_caster = caster;
		m_ability = ability;

		//show the ui
		TargetingPrompt.SetActive(true);
		m_canceled = false;

		// wait for a unit to be selected
		while(m_selectUnitMessage.Idle && ! m_canceled)
		{
			yield return 0;
		}

		//hide the ui
		TargetingPrompt.SetActive(false);

		// if we recived a message (not the cancel message)
		if(m_selectUnitMessage.MessagePending)
		{
			yield return new Routine(m_selectUnitMessage.WaitHandleMessage(WaitHandleUnitSelected));
		}
	}

	private bool m_canceled = false;
	private Unit m_caster;
	private UnitAbility m_ability;
	//supplyed in Init
	private Message<Unit> m_selectUnitMessage;
	private CombatLogic m_logic;

}

