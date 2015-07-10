using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityGizmos = UnityEngine.Gizmos;

namespace Horizon.Core.Gizmos
{
	public static class MeshExtension
	{
		public static Mesh TransformMesh(this Mesh self, Transform transform)
		{
			List<Vector3> newVerts =  new List<Vector3>();
			foreach(Vector3 vert in self.vertices)
			{
				newVerts.Add (transform.TransformVector(vert) + transform.position);
			}

			Mesh m = Object.Instantiate(self);
			m.vertices = newVerts.ToArray();
			return m;
		}
	}

	public class HorizonGizmos 
	{
		static HorizonGizmos()
		{
			m_rectangleMesh = new Mesh();
			m_rectangleMesh.hideFlags = HideFlags.DontSave;
			m_rectangleMesh.vertices = new Vector3[]{ 
				new Vector3(0.5f,0.0f,0.5f), 
				new Vector3(-0.5f,0.0f,0.5f), 
				new Vector3(-0.5f,0.0f,-0.5f), 
				new Vector3(0.5f,0.0f,-0.5f)
			};
			
			m_rectangleMesh.triangles = new int[]{
				2,1,0,
				3,2,0
			};
			
			m_rectangleMesh.normals = new Vector3[]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
		}

		public static void DrawSolidRectangleGizmo(Transform transform, Color color)
		{
#if UNITY_EDITOR
			UnityGizmos.color = color;
			UnityGizmos.DrawMesh(m_rectangleMesh.TransformMesh(transform));
#endif
		}

		private static Mesh m_rectangleMesh;
	}
}

