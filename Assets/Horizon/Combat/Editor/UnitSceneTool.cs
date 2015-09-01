using UnityEngine;
using System.Collections;
using Horizon.Core.Editor;
using Horizon.Combat.Models;
using UnityEditor;
using System.Linq;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Combat.Editor
{
	public class UnitSceneTool : SceneView<Unit>
	{
		bool move = true;
		
		public override void OnSceneGUI ()
		{
			base.OnSceneGUI ();
			
			if(model.grid == null) return;

			if(Tools.current == Tool.Move)
				move = true;
			else if(Tools.current == Tool.Rotate)
				move = false;

			Tools.current = Tool.None;

			if(move)
			{
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
			}
			else
			{
				Handles.color = Color.white.SetAlpha(0.25f);
				Handles.DrawWireDisc(model.transform.position,model.transform.up, 0.75f);

				Handles.color = Color.white;
				if(Handles.Button(model.transform.position + Vector3.forward * 0.75f, GridDirection.North.Rotation() ,0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.North;
				if(Handles.Button(model.transform.position + Vector3.right   * 0.75f, GridDirection.East.Rotation()  ,0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.East;
				if(Handles.Button(model.transform.position + Vector3.back    * 0.75f, GridDirection.South.Rotation() ,0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.South;
				if(Handles.Button(model.transform.position + Vector3.left    * 0.75f, GridDirection.West.Rotation()  ,0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.West;
			}
		}
	}
}
