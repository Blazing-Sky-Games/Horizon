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
				Vector3 dragVec = Handles.Slider2D(model.transform.position,Vector3.up,Vector3.forward,Vector3.right,0.3f,Handles.SphereCap,1);

				if(dragVec.z - model.transform.position.z > model.grid.CellSize / 2.0f)
					model.GridPosition = new GridPoint(model.GridPosition.x,model.GridPosition.y+1);
				else if(dragVec.z - model.transform.position.z < model.grid.CellSize / 2.0f * -1)
					model.GridPosition = new GridPoint(model.GridPosition.x,model.GridPosition.y-1);

				if(dragVec.x - model.transform.position.x > model.grid.CellSize / 2.0f)
					model.GridPosition = new GridPoint(model.GridPosition.x+1,model.GridPosition.y);
				else if(dragVec.x - model.transform.position.x < model.grid.CellSize / 2.0f * -1)
					model.GridPosition = new GridPoint(model.GridPosition.x-1,model.GridPosition.y);
			}
			else
			{
				Handles.color = Color.white.SetAlpha(0.1f);
				Handles.DrawWireDisc(model.transform.position,model.transform.up,model.grid.CellSize * 0.75f);

				Handles.color = Color.white;
				if(Handles.Button(model.transform.position + Vector3.forward * model.grid.CellSize * 0.75f, Quaternion.identity,0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.North;
				if(Handles.Button(model.transform.position + Vector3.right * model.grid.CellSize * 0.75f, Quaternion.FromToRotation(Vector3.forward,Vector3.right),0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.East;
				if(Handles.Button(model.transform.position + Vector3.back * model.grid.CellSize * 0.75f, Quaternion.AngleAxis(180,Vector3.up),0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.South;
				if(Handles.Button(model.transform.position + Vector3.left * model.grid.CellSize * 0.75f, Quaternion.FromToRotation(Vector3.forward,Vector3.left),0.1f,0.1f,Handles.ConeCap)) model.DirectionFacing = GridDirection.West;
			}
		}
	}
}
