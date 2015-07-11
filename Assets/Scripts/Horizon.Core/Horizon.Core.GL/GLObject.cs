using System;
using Horizon.Core;
using Horizon.Core.WeakSubscription;
using UnityEngine;
namespace Horizon.Core.GL
{
	public abstract class GLObject : IDisposable
	{
		//TODO creat a way to "parent" a globject to a transform
		public virtual void Draw()
		{
			if(m_drawingMaterial == null)
			{
				m_drawingMaterial = GLUtility.DefaultMaterial;

				//update the drawing material with the settings
				Settings = Settings;
			}

			m_drawingMaterial.SetPass(0);
		}

		public GLSettings Settings
		{
			get
			{
				return m_settings;
			}
			set
			{
				m_settings = value;

				if(m_drawingMaterial != null)
				{
					m_drawingMaterial.SetColor("_Color", m_settings.color);
					m_drawingMaterial.SetInt ("_SrcBlend", (int)m_settings.SrcBlend);
					m_drawingMaterial.SetInt ("_DstBlend", (int)m_settings.DstBlend);
					m_drawingMaterial.SetInt ("_Cull", (int)m_settings.CullMode);
					m_drawingMaterial.SetInt ("_ZWrite", m_settings.ZWrite ? 1 : 0);
					m_drawingMaterial.SetInt ("_ZTest", (int)m_settings.ZTest);
				}
			}
		}

		public bool Enabled;

		public GLObject()
		{
			m_postRenderSubscription = HorizonCamera.Main.WeakSubscribeToEvent(
				HorizonCamera.Main.PostRenderEventName, 
				(sender,args) => 
				{
					if(Enabled)
					{
						Draw();
					}
				}
			);

			Enabled = true;
		}

		public void Dispose()
		{
			m_postRenderSubscription.Dispose();
		}

		private IDisposable m_postRenderSubscription;
		private GLSettings m_settings;
		private Material m_drawingMaterial;
	}
}

