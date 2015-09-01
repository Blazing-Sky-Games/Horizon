using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Core.Editor.Gizmos
{
	//rectangle highlight in sceneview
	public class RectangleGizmo : Gizmo
	{
		static GameObject Temp
		{
			get
			{
				if(m_temp == null)
				{
					m_temp = new GameObject();
					m_temp.hideFlags = HideFlags.HideAndDontSave;
				}

				return m_temp;
			}

		}

		public RectangleGizmo(ModelBase gameObject) : base(gameObject){}

		public float Size = 1;

		protected override void Draw ()
		{
			base.Draw();
			UnityEngine.Gizmos.DrawMesh(rectangleMesh,gameObject.transform.position,gameObject.transform.rotation,gameObject.transform.localScale * Size);
		}

		public override void Dispose ()
		{
			base.Dispose ();

			if(m_rectangleMesh != null)
				UnityEngine.Object.DestroyImmediate(m_rectangleMesh);
		}

		private Mesh rectangleMesh
		{
			get
			{
				if(m_rectangleMesh == null)
				{
					m_rectangleMesh = new Mesh();
					m_rectangleMesh.hideFlags = HideFlags.HideAndDontSave;
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

		private static Mesh m_rectangleMesh;
		private static GameObject m_temp;
	}
}

