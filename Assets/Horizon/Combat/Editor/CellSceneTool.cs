using UnityEngine;
using System.Collections;
using Horizon.Core.Editor;
using Horizon.Combat.Models;
using UnityEditor;
using System.Linq;

namespace Horizon.Combat.Editor
{
	public class CellSceneTool : SceneView<Cell>
	{
		public override void OnSceneGUI ()
		{
			base.OnSceneGUI ();

			Tools.current = Tool.None;
		}
	}
}


