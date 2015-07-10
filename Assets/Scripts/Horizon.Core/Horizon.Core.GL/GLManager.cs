using System;
using System.Collections.Generic;
using UnityEngine;

namespace Horizon.Core.GL
{
	public class GLManager
	{
		static GLManager ()
		{
			m_glObjects = new List<GLObject>();
			m_drawingMaterial = GLUtility.DefaultMaterial;
		}

		public static void DrawGLObjects()
		{
			foreach(GLObject glObject in m_glObjects)
			{
				if(glObject.Enabled)
				{
					updateSettings(glObject.Settings);
					m_drawingMaterial.SetPass(0);
					glObject.Draw();
				}
			}
		}

		public static void __addGLObject(GLObject glObject)
		{
			m_glObjects.Add(glObject);
		}

		public static void __removeGLObject(GLObject glObject)
		{
			m_glObjects.Remove(glObject);
		}

		private static List<GLObject> m_glObjects;
		private static Material m_drawingMaterial;

		private static void updateSettings(GLSettings settings)
		{
			m_drawingMaterial.SetColor("_Color", settings.color);
			m_drawingMaterial.SetInt ("_SrcBlend", (int)settings.SrcBlend);
			m_drawingMaterial.SetInt ("_DstBlend", (int)settings.DstBlend);
			m_drawingMaterial.SetInt ("_Cull", (int)settings.CullMode);
			m_drawingMaterial.SetInt ("_ZWrite", settings.ZWrite ? 1 : 0);
			m_drawingMaterial.SetInt ("_ZTest", (int)settings.ZTest);
		}
	}
}

