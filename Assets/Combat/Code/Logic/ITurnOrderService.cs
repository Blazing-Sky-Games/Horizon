using System.Collections;
using System.Collections.Generic;

public interface ITurnOrderService : IService
{
	Unit ActiveUnit { get; }

	IEnumerator WaitAdvance ();

	void SetOrder (IEnumerable<Unit> ids);
}