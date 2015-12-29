using UnityEngine;
using System.Collections;

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
		yield break;
	}

	public string ActorName
	{
		get
		{
			return m_actorName;
		}
	}

	public Actor(string name)
	{
		m_actorName = name;
	}

	public IEnumerator WaitPassTurn()
	{
		m_passedTurn = true;
		yield return ActionDecidedMessage.WaitSend(new PassTurnAction());
	}
	
	public IEnumerator WaitUseUnitAbility(Unit caster, UnitAbility ability, Unit target)
	{
		m_usedAction = true;
		yield return ActionDecidedMessage.WaitSend(new UnitAbilityUsageAction(caster, ability, target));
	}

	private readonly string m_actorName;
	private bool m_passedTurn = false;
	private bool m_usedAction = false;
}
