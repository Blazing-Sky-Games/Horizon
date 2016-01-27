using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


namespace Combat.Code.Services.TurnOrderService
{
	public interface ITurnOrderService : IService
	{
		Unit ActiveUnit { get; }
		IEnumerator WaitAdvance();
	}
}

