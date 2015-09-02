using System;
using Horizon.Core;
using Horizon.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Horizon.Core.Editor
{
	[CustomDrawerAtribute(typeof(AnimatedMesh))]
	public class AnimatedMeshDrawer : CustomDrawer
	{
		// user selects value for the mesh
		public override object Draw (string label, object val)
		{
			if (val != null)
			{
				AnimatedMesh mesh = (AnimatedMesh)val;

				mesh.mesh = (GameObject)EditorGUILayout.ObjectField(label, mesh.mesh,typeof(GameObject),false);
			}

			return val;
		}
	}
}


