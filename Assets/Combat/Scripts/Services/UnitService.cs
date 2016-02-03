using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class UnitService : Service, IUnitService
{
	private readonly Message<Unit> m_unitDied = new Message<Unit>();

	public Message<Unit> UnitDied
	{
		get
		{
			return m_unitDied;
		}
	}

	public IEnumerable<Unit> UnitsOfFaction (Faction faction)
	{
		return m_units.Where(unit => unit.Faction == faction);
	}
	public IEnumerable<Unit> CreateUnits (IEnumerable<UnitData> units)
	{
		List<Unit> created = new List<Unit>();
		foreach(UnitData data in units)
		{
			created.Add(CreateUnit(data));
		}
		return created;
	}
	public Unit CreateUnit (UnitData data)
	{
		Unit created = new Unit(data);

		created.Health.Zero.AddHandler(GetUnitDiedHandler(created));

		m_units.Add(created);
		return created;
	}

	private Func<IEnumerator> GetUnitDiedHandler(Unit unit)
	{
		if(!DiedHandlers.ContainsKey(unit))
		{
			DiedHandlers[unit] = () => WaitKillUnit(unit);
		}

		return DiedHandlers[unit];
	}

	private IEnumerator WaitKillUnit(Unit unit)
	{
		DiedHandlers.Remove(unit);
		m_units.Remove(unit);
		ServiceLocator.GetService<ILoggingService>().Log("unit " + unit.Name + " died");
		yield return new Routine(UnitDied.WaitSend(unit));
	}

	private Dictionary<Unit, Func<IEnumerator>> DiedHandlers = new Dictionary<Unit, Func<IEnumerator>>();

	private List<Unit> m_units = new List<Unit>();
}


