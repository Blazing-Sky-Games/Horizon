using System;
using System.Collections;

public interface IGenericMessage
{
	IEnumerator WaitSendGeneric (object[] args);
	void AddHandlerGeneric(Func<object[],IEnumerator> handler);
	void RemoveHandlerGeneric(Func<object[],IEnumerator> handler);
}
