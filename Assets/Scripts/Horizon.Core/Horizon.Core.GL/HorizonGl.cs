using UnityEngine;
using System.Collections;

namespace Horizon.Core.Gl
{
	public static class ColorExtentions
	{
		public static Color SetAlpha(this Color self, float alpha)
		{
			return new Color(self.r,self.g,self.b,alpha);
		}
	}

	public struct HorizonGLSettings
	{
		public readonly Color color;
		public readonly UnityEngine.Rendering.BlendMode SrcBlend;
		public readonly UnityEngine.Rendering.BlendMode DstBlend;
		public readonly UnityEngine.Rendering.CullMode CullMode;
		public readonly bool ZWrite;
		public readonly UnityEngine.Rendering.CompareFunction ZTest;

		public HorizonGLSettings(
			Color color = default(Color),
			UnityEngine.Rendering.BlendMode SrcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha,
			UnityEngine.Rendering.BlendMode DstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
			UnityEngine.Rendering.CullMode  CullMode = UnityEngine.Rendering.CullMode.Off,
			bool ZWrite = true,
			UnityEngine.Rendering.CompareFunction ZTest = UnityEngine.Rendering.CompareFunction.LessEqual
		)
		{
			this.color = color;
			this.SrcBlend = SrcBlend;
			this.DstBlend = DstBlend;
			this.CullMode = CullMode;
			this.ZWrite = ZWrite;
			this.ZTest = ZTest;
		}

		public static HorizonGLSettings DefaultSettings
		{
			get
			{
				return new HorizonGLSettings();
			}
		}
	}

	public class HorizonGL
	{

		static HorizonGL()
		{
			// Unity has a built-in shader that is useful for drawing simple colored things.			
			var shader = Shader.Find ("Hidden/Internal-Colored");
			var mat = new Material (shader);
			mat.hideFlags = HideFlags.HideAndDontSave;
			m_drawingMaterial = mat;

			Settings = HorizonGLSettings.DefaultSettings;
		}

		public static HorizonGLSettings Settings
		{
			get
			{
				return m_settings;
			}
			set
			{
				m_settings = value;


				m_drawingMaterial.SetColor("_Color", m_settings.color);
				m_drawingMaterial.SetInt ("_SrcBlend", (int)m_settings.SrcBlend);
				m_drawingMaterial.SetInt ("_DstBlend", (int)m_settings.DstBlend);
				m_drawingMaterial.SetInt ("_Cull", (int)m_settings.CullMode);
				m_drawingMaterial.SetInt ("_ZWrite", m_settings.ZWrite ? 1 : 0);
				m_drawingMaterial.SetInt ("_ZTest", (int)m_settings.ZTest);
			}
		}
		
		public static void DrawLine(Vector3 a, Vector3 b)
		{
			m_drawingMaterial.SetPass( 0 );
			GL.Begin( GL.LINES );
			
			GL.Vertex(a);
			GL.Vertex(b);
			
			GL.End();
		}

		public static void DrawLine(Vector3 a, Vector3 b, HorizonGLSettings overideSettings)
		{
			HorizonGLSettings oldSettings = Settings;
			Settings = overideSettings;

			m_drawingMaterial.SetPass( 0 );
			GL.Begin( GL.LINES );
			
			GL.Vertex(a);
			GL.Vertex(b);
			
			GL.End();

			Settings = oldSettings;
		}

		private static HorizonGLSettings m_settings;
		private static Material m_drawingMaterial;
	}
}


