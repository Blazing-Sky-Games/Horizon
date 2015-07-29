using System;
using System.Collections.Generic;
using UnityEngine;

namespace Horizon.Core.ExtensionMethods
{
	public static class MeshExtensionMethods
	{
		public static Mesh TransformMesh(this Mesh self, Transform transform)
		{
			List<Vector3> newVerts =  new List<Vector3>();
			foreach(Vector3 vert in self.vertices)
			{
				newVerts.Add (transform.TransformVector(vert) + transform.position);
			}
			
			Mesh m = UnityEngine.Object.Instantiate(self);
			//m.hideFlags = HideFlags.HideAndDontSave;
			m.vertices = newVerts.ToArray();
			return m;
		}
	}
}

