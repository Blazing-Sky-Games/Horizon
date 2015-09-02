using System;
using UnityEngine;
using Horizon.Combat.Models;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.WeakSubscription;
using Horizon.Core.GL;


namespace Horizon.Combat.Views
{
	public class CellGameHighlight : ViewBase<Cell>
	{
		//private SpriteRenderer cellSpriteRenderer;
		//private static Sprite cellSprite;

		protected override void Init ()
		{
			//if(cellSprite == null)
			//{
				//cellSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1),new Vector2(0.5f,0.5f),1);
			//}

			//cellSpriteRenderer = model.GetComponentInChildren<SpriteRenderer>();

			//if(cellSpriteRenderer == null)
			//{
				//Debug.LogError("cell must have a sprite renderer attached");
				//return;
			//}

			//if(cellSpriteRenderer.sprite == null)
				//cellSpriteRenderer.sprite = cellSprite;

			//cellSpriteRenderer.color = GetHighlightColor(model.HighlightState.EffectiveHighlightState());
		}

		public override void Update ()
		{
			base.Update ();

			//cellSpriteRenderer.color = GetHighlightColor(model.HighlightState.EffectiveHighlightState());
		}

		//Color GetHighlightColor (LogicalHighlightState logicalHighlightState)
		//{
			//switch(logicalHighlightState)
			//{
			//case(LogicalHighlightState.MovementRange): return Color.blue.SetAlpha(0.25f);
			//case(LogicalHighlightState.MovementPath): return Color.green.SetAlpha(0.25f);
			//case(LogicalHighlightState.TargetingRange): return Color.red.SetAlpha(0.25f);
			//case(LogicalHighlightState.EffectRange): return Color.yellow.SetAlpha(0.25f);
			//case(LogicalHighlightState.None):
			//default: return Color.clear;
			//}
		//}
	}
}

