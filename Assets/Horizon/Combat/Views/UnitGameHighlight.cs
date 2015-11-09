using System;
using System.Linq;
using UnityEngine;
using Horizon.Combat.Models;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;

namespace Horizon.Combat.Views
{
	public class UnitGameHighlight : ViewBase<Unit>
	{
		protected override void Init ()
		{
			model.WeakSubscribe(() => model.Highlighted, 
			(oldval, newval) => 
			{
				model.gameObject.SetLayerRecursively(newval ? 9 : 0);
			});
		}
	}
}

