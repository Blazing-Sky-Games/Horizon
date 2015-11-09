using UnityEngine;
using System.Collections;
using Horizon.Core.Editor;
using Horizon.Combat.Models;
using UnityEditor;
using System.Linq;
using Horizon.Core.ExtensionMethods;
using Horizon.Core;

namespace Horizon.Combat.Editor
{
	//unit positioning tools for the scene view
	public class UnitSceneTool : ViewBase<Unit>
	{
		//is the move tool selected
		bool move = true;
		
		public override void OnSceneGUI ()
		{
			base.OnSceneGUI ();
			
			//no grid! jump ship!
			if(model.grid == null) return;

			//dont shopw the unity move/rotate tools, but keep tract of what is selected
			if(Tools.current == Tool.Move)
				move = true;
			else if(Tools.current == Tool.Rotate)
				move = false;

			Tools.current = Tool.None;

			if(move)
			{
				//draw a dragable sphear. user click and drag sphear to move unit
				Handles.color = Color.white.SetAlpha(0.25f);
				Vector3 dragVec = Handles.Slider2D(model.transform.position,Vector3.up,Vector3.forward,Vector3.right,0.3f,Handles.SphereCap,1);

				if(dragVec.z - model.transform.position.z > 0.5f)
					model.GridPosition = new GridPoint(model.GridPosition.x,model.GridPosition.y+1);
				else if(dragVec.z - model.transform.position.z < -0.5f)
					model.GridPosition = new GridPoint(model.GridPosition.x,model.GridPosition.y-1);

				if(dragVec.x - model.transform.position.x > 0.5f)
					model.GridPosition = new GridPoint(model.GridPosition.x+1,model.GridPosition.y);
				else if(dragVec.x - model.transform.position.x < -0.5f)
					model.GridPosition = new GridPoint(model.GridPosition.x-1,model.GridPosition.y);

				EditorUtility.SetDirty(model);
			}
			else
			{
				//draw compase control. click on arrow to rotate unit
				Handles.color = Color.white.SetAlpha(0.25f);
				Handles.DrawWireDisc(model.transform.position,model.transform.up, 0.75f);

				Handles.color = Color.white;
				if(Handles.Button(model.transform.position + Vector3.forward * 0.75f, GridDirection.North.Rotation() ,0.1f,0.1f,Handles.ConeCap)) {model.DirectionFacing = GridDirection.North; EditorUtility.SetDirty(model);}
				if(Handles.Button(model.transform.position + Vector3.right   * 0.75f, GridDirection.East.Rotation()  ,0.1f,0.1f,Handles.ConeCap)) {model.DirectionFacing = GridDirection.East; EditorUtility.SetDirty(model);}
				if(Handles.Button(model.transform.position + Vector3.back    * 0.75f, GridDirection.South.Rotation() ,0.1f,0.1f,Handles.ConeCap)) {model.DirectionFacing = GridDirection.South; EditorUtility.SetDirty(model);}
				if(Handles.Button(model.transform.position + Vector3.left    * 0.75f, GridDirection.West.Rotation()  ,0.1f,0.1f,Handles.ConeCap)) {model.DirectionFacing = GridDirection.West; EditorUtility.SetDirty(model);}
			}
		}
	}
}
