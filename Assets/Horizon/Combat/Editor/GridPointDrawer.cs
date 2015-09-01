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
using Horizon.Core.Editor;
using UnityEditor;
using UnityEngine;


namespace Horizon.Combat.Editor
{
	//display a grid point
	[CustomDrawerAtribute(typeof(GridPoint))]
	public class GridPointDrawer : CustomDrawer
	{
		public override object Draw (string label, object val)
		{
			GridPoint value = (GridPoint)val;
			int x;
			int y;

			EditorGUILayout.LabelField(label);
			GUIStyle SplitterStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.whiteTexture},
				stretchWidth = false,
				margin = new RectOffset(25, 0, 0, 0)
			};

			Color old = GUI.color;

			//bug, this needs to change during play mode
			GUI.color = new Color(0.85f, 0.85f, 0.85f);
			EditorGUILayout.BeginVertical(SplitterStyle);
			x = EditorGUILayout.IntField("X",value.x,GUILayout.ExpandWidth(false));
			y = EditorGUILayout.IntField("Y",value.y,GUILayout.ExpandWidth(false));
			EditorGUILayout.EndVertical();
			GUI.color = old;

			return new GridPoint(x,y);
		}
	}
}


