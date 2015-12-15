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

	public virtual void DecideAction()
	{
		//empty
		//this is where an AI would decide what to do
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

	public void PassTurn ()
	{
		passedTurn = true;
		ActionDecidedMessage.SendMessage (new PassTurnAction ());
		// TODO wait for ActionDecidedMessage
	}
	
	public void UseUnitAbility (Unit caster, UnitAbility ability, Unit target)
	{
		UsedAction = true;
		ActionDecidedMessage.SendMessage (new UnitAbilityUsageAction (caster, ability, target));
		// TODO wait for ActionDecidedMessage
	}

	string m_actorName;
	bool passedTurn;
	bool UsedAction;
}
