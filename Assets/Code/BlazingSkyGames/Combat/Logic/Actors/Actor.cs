/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections;

public class Actor
{
	public readonly string DebugName;

	public Actor(string debugName)
	{
		DebugName = debugName;
	}

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
			//TODO fix this so it doesnt have to hang
			yield return new WaitForNextUpdate();
		}
	}

	public IEnumerator WaitPassTurn()
	{
		m_passedTurn = true;
		//ServiceLocator.GetService<ILoggingService>().Log("actor " + DebugName + " passes turn");
		yield return new Routine(ActionDecidedMessage.WaitSend(new PassTurnAction()));
	}
	
	public IEnumerator WaitUseUnitAbility(Unit caster, UnitAbility ability, Unit target)
	{
		m_usedAction = true;
		//ServiceLocator.GetService<ILoggingService>().Log("actor " + DebugName + " used unit ability");
		yield return new Routine(ActionDecidedMessage.WaitSend(new UnitAbilityUsageAction(caster, ability, target)));
	}

	private bool m_passedTurn = false;
	private bool m_usedAction = false;
}
