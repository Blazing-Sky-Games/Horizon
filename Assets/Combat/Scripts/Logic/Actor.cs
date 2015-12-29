using UnityEngine;
using System.Collections;

public class Actor
{
	public readonly MessageChannel<IActorAction> ActionDecidedMessage = new MessageChannel<IActorAction> ();

	public bool CanTakeAction
	{
		get
		{
			return !passedTurn && !UsedAction;
		}
	}

	public void ResetCanTakeAction ()
	{
		passedTurn = false;
		UsedAction = false;
	}

	public virtual IEnumerator DecideAction()
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

	public Actor (string name)
	{
		m_actorName = name;
	}

	public IEnumerator PassTurn ()
	{
		passedTurn = true;
		yield return ActionDecidedMessage.Send (new PassTurnAction ());
	}
	
	public IEnumerator UseUnitAbility (Unit caster, UnitAbility ability, Unit target)
	{
		UsedAction = true;
		yield return ActionDecidedMessage.Send(new UnitAbilityUsageAction (caster, ability, target));
	}

	string m_actorName;
	bool passedTurn;
	bool UsedAction;
}
