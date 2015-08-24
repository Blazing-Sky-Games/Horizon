//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using Horizon.Combat.Models;
using Horizon.Core.Editor;
using Horizon.Core.Editor.Gizmos;
using UnityEngine;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.WeakSubscription;
using UnityEditor;


namespace Horizon.Combat.Editor
{
	public class CellSelectedLabel : SceneView<Cell>
	{
		protected override void WhileSelected ()
		{
			base.WhileSelected ();

			if (string.IsNullOrEmpty(model.name)) return;
			var color = GUI.color;
			GUI.color = Color.white;
			
			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(42/255f, 192/255f, 217/255f, 0.5f);
			
			GUIStyle DefaultLabelStyle = new GUIStyle
			{
				normal =
				{
					background = EditorGUIUtility.whiteTexture,
					textColor = Color.white
				},
				
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0),
				alignment = TextAnchor.MiddleCenter,
				border = new RectOffset(6, 6, 6, 6),
				fontSize = 12
			};

			Handles.Label(model.transform.position, model.name, DefaultLabelStyle);
			
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}
	}
}
