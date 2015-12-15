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
	public void Init(MessageChannel<Unit> SelectUnitMessage, CombatLogic Logic)
	{
		m_selectUnitMessage = SelectUnitMessage;
		m_logic = Logic;

		Cancel.onClick.AddListener (OnCancleClick);
	}

	// send cancel message if the user backs out of selecting a unit
	void OnCancleClick ()
	{
		m_selectUnitMessage.SendCancel ();
	}

	// call this to bring up the ui and select a target for an ability
	public Coroutine WaitSelectTarget (Unit caster, UnitAbility ability)
	{
		return StartCoroutine (WaitSelectTargetRoutine (caster, ability));
	}

	// called by WaitSelectTarget
	private IEnumerator WaitSelectTargetRoutine (Unit caster, UnitAbility ability)
	{
		//show the ui
		TargetingPrompt.SetActive (true);

		// wait for a unit to be selected
		yield return m_selectUnitMessage.WaitForMessage (); 

		//hide the ui
		TargetingPrompt.SetActive (false);

		// if we recived a message (not the cancel message)
		if (m_selectUnitMessage.State == MessageChannelState.MessagePending)
		{
			m_selectUnitMessage.BeginProccesMessage();

				// the user has selected a target, so declare that they have used an ability
				m_logic.GetFactionLeader (caster.Faction).UseUnitAbility (caster, ability, m_selectUnitMessage.MessageContent);
				//wait for the message to be processed
				yield return m_logic.GetFactionLeader (caster.Faction).ActionDecidedMessage.WaitTillMessageProcessed ();

			m_selectUnitMessage.EndProccesMessage();

			yield return m_selectUnitMessage.WaitTillMessageProcessed();
		}
	}

	//supplyed in Init
	private MessageChannel<Unit> m_selectUnitMessage;
	private CombatLogic m_logic;
}

