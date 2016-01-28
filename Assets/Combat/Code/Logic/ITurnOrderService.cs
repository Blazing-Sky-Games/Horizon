using System.Collections;
using System.Collections.Generic;

public interface ITurnOrderService : IService
{
	UnitId ActiveUnitId { get; }

	IEnumerator WaitAdvance ();

	void SetOrder (IEnumerable<UnitId> ids);
}