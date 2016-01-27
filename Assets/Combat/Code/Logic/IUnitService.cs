using UnityEngine;
using System;
using System.Linq;
using Combat.Code.Services.TurnOrderService;
using Combat.Code.Services.FactionService;
using System.Collections.Generic;

interface IUnitService : IService
{
	IEnumerable<Unit> UnitsOfFaction (Faction faction);
}

