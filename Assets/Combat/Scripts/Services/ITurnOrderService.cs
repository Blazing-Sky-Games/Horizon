using System.Collections;
using System.Collections.Generic;

public interface ITurnOrderService : IService
{
	Message TurnOrderAdvanced { get; }

	Message OrderChanged { get; }

	Unit ActiveUnit { get; }

	IEnumerator WaitAdvance ();

	void SetOrder (IEnumerable<Unit> ids);
}