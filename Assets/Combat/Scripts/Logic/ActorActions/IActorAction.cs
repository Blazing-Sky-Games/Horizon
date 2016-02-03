using System.Collections;

public interface IActorAction
{
	// perform the action the actor decided and wait for it to finish
	IEnumerator WaitPerform();
}
