using UnityEngine;
using System.Collections;
using Horizon.Core.Editor;
using Horizon.Combat.Models;
using UnityEditor;
using System.Linq;

namespace Horizon.Combat.Editor
{
	public class UnitSceneTool : SceneView<Unit>
	{
		bool move;
		
		public override void OnSceneGUI ()
		{
			base.OnSceneGUI ();
			
			if(Tools.current == Tool.Move)
				move = true;
			else if(Tools.current == Tool.Rotate)
				move = false;



			Tools.current = Tool.None;

			Handles.color = Color.blue;
			float dragY = Handles.Slider(model.transform.position,model.grid.transform.forward).z - model.transform.position.z;
			//Debug.Log("dragy " + dragY);
			if(dragY > model.grid.CellSize)
				model.Position = new GridPoint(model.Position.x,model.Position.y+1);
			else if(dragY < model.grid.CellSize * -1)
				model.Position = new GridPoint(model.Position.x,model.Position.y-1);

			Handles.color = Color.red;
			float dragX = Handles.Slider(model.transform.position,model.grid.transform.right).x - model.transform.position.x;
			//Debug.Log("dragx " + dragX);
			if(dragX > model.grid.CellSize)
				model.Position = new GridPoint(model.Position.x+1,model.Position.y);
			else if(dragX < model.grid.CellSize * -1)
				model.Position = new GridPoint(model.Position.x-1,model.Position.y);
		}
	}
}
