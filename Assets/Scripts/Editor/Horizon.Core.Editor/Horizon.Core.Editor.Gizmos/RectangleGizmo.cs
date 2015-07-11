using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Core.Editor.Gizmos
{
	public class RectangleGizmo : Gizmo
	{
		public RectangleGizmo(HorizonGameObjectBase gameObject) : base(gameObject){}

		protected override void Draw ()
		{
			base.Draw();
			UnityEngine.Gizmos.DrawMesh(rectangleMesh.TransformMesh(gameObject.transform));
		}

		private Mesh rectangleMesh
		{
			get
			{
				if(m_rectangleMesh == null)
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

				return m_rectangleMesh;
			}
		}

		private Mesh m_rectangleMesh;
	}
}

