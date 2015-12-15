using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetingUI : MonoBehaviour
{
	//supplyed in editor
	public GameObject TargetingPrompt;
	public Button Cancel;
	public IActorAction Result;

	public void Init(MessageChannel<Unit> SelectUnitMessage, CombatLogic Logic)
	{
		m_selectUnitMessage = SelectUnitMessage;
		m_logic = Logic;

		Cancel.onClick.AddListener (OnCancleClick);
	}

	void OnCancleClick ()
	{
		m_selectUnitMessage.SendCancel ();
	}

	public Coroutine WaitSelectTarget (Unit caster, UnitAbility ability)
	{
		return StartCoroutine (WaitSelectTargetRoutine (caster, ability));
	}

	private IEnumerator WaitSelectTargetRoutine (Unit caster, UnitAbility ability)
	{
		TargetingPrompt.SetActive (true);

		yield return m_selectUnitMessage.WaitForMessage (); 

		TargetingPrompt.SetActive (false);

		if (m_selectUnitMessage.State == MessageChannelState.MessagePending)
		{
			m_logic.GetFactionLeader (caster.Faction).UseUnitAbility (caster, ability, m_selectUnitMessage.Content);
			yield return m_logic.GetFactionLeader (caster.Faction).ActionDecidedMessage.WaitTillMessageProcessed ();
		}
	}

	//supplyed in Init
	private MessageChannel<Unit> m_selectUnitMessage;
	private CombatLogic m_logic;
}

