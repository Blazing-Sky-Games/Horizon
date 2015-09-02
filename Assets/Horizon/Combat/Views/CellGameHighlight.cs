using System;
using UnityEngine;
using Horizon.Combat.Models;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.WeakSubscription;
using Horizon.Core.GL;
using System.Linq;


namespace Horizon.Combat.Views
{
	public class CellGameHighlight : ViewBase<Cell>
	{
		private SpriteRenderer cellSpriteRenderer;

		protected override void Init ()
		{
			cellSpriteRenderer = model.GetComponentsInChildren<SpriteRenderer>(true).First();

			cellSpriteRenderer.color = GetHighlightColor(model.HighlightState.EffectiveHighlightState());
			model.WeakSubscribe(() => model.HighlightState, 
			(s, e) => 
			{
				cellSpriteRenderer.color = GetHighlightColor(model.HighlightState.EffectiveHighlightState());
			});
		}

		Color GetHighlightColor (LogicalHighlightState logicalHighlightState)
		{
			switch(logicalHighlightState)
			{
			case(LogicalHighlightState.MovementRange): return Color.cyan;
			case(LogicalHighlightState.MovementPath): return Color.green;
			case(LogicalHighlightState.TargetingRange): return Color.red;
			case(LogicalHighlightState.EffectRange): return Color.yellow;
			default: return Color.clear;
			}
		}
	}
}

