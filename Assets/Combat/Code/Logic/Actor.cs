using UnityEngine;
using System.Collections;

//TODO do we really need "actors"? or can each unit be an "actor"
public class Actor
{
	public readonly Message<IActorAction> ActionDecidedMessage = new Message<IActorAction>();

	public bool CanTakeAction
	{
		get
		{
			return !m_passedTurn && !m_usedAction;
		}
	}

	public void ResetCanTakeAction()
	{
		m_passedTurn = false;
		m_usedAction = false;
	}

	public virtual IEnumerator WaitDecideAction()
	{
		bool oldPassedTurn = m_passedTurn;
		bool oldUsedAction = m_usedAction;

		while(oldPassedTurn == m_passedTurn && oldUsedAction == m_usedAction)
		{
			yield return new WaitForNextUpdate();
		}
	}

	public IEnumerator WaitPassTurn()
	{
		m_passedTurn = true;
		yield return new Routine(ActionDecidedMessage.WaitSend(new PassTurnAction()));
	}
	
	public IEnumerator WaitUseUnitAbility(Unit caster, UnitAbility ability, Unit target)
	{
		m_usedAction = true;
		yield return new Routine(ActionDecidedMessage.WaitSend(new UnitAbilityUsageAction(caster, ability, target)));
	}

	private bool m_passedTurn = false;
	private bool m_usedAction = false;
}
