using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


namespace Combat.Code.Services.TurnOrderService
{
	public interface ITurnOrderService : IService
	{
		UnitId ActiveUnitId { get; }
		IEnumerator WaitAdvance();

		void SetOrder (IEnumerable<UnitId> ids);
	}
}

