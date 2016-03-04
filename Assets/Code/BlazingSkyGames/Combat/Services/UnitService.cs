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


