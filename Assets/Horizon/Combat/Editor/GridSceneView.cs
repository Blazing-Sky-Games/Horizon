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
using UnityEngine;
using UnityEditor;


namespace Horizon.Combat.Editor
{
	public class GridSceneView : SceneView<Grid>
	{
		protected override void SceneViewUpdate ()
		{
			base.SceneViewUpdate ();

			Handles.color = Handles.color*new Color(1,1,1,0.25f);

			foreach(GridLine line in model.GridLines)
			{
				Handles.DrawDottedLine(line.start,line.end,4);
			}
		}
	}
}

