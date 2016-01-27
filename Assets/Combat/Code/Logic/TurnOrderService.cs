using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Combat.Code.Services.TurnOrderService
{
	// list of units. the order in which unts take their turn
	public class TurnOrderService : ITurnOrderService, IEnumerable<UnitId>
	{
		public void SetOrder (IEnumerable<UnitId> ids)
		{
			m_units = ids.ToList();
		}

		public UnitId ActiveUnitId
		{
			get
			{
				return m_units[ActiveUnitIndex];
			}
		}

		public void LoadService ()
		{
			throw new NotImplementedException();
		}

		public void UnloadService ()
		{
			throw new NotImplementedException();
		}

		public readonly Message AdvanceTurnOrderMessage = new Message();
		public readonly Message<bool> CombatEncounterOverMessage = new Message<bool>();
		public readonly Message<Unit> UnitKilledMessage = new Message<Unit>();

		public int ActiveUnitIndex
		{
			get
			{
				return m_activeUnitIndex;
			}
		}

		//for IEnumerable<UnitId>
		public IEnumerator<UnitId> GetEnumerator ()
		{
			return m_units.GetEnumerator();
		}

		//for IEnumerable<Unit>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator WaitAdvance ()
		{
			m_activeUnitIndex++;
			m_activeUnitIndex %= m_units.Count;

			yield return new Routine(AdvanceTurnOrderMessage.WaitSend());
		}

		private List<UnitId> m_units;
		private int m_activeUnitIndex = 0;
	}

}
