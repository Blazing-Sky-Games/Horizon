using System;
using System.Linq;
using Horizon.Core;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;
using UnityEngine;
using UnityEngine.Rendering;
using Horizon.Combat.Models;

namespace Horizon.Core
{
	public class CombatCamera : HorizonCamera
	{
		private RenderTexture ScreenSpaceUnitIdRenderTexture;
		private Shader screenSpaceIdWrite;
		private Material screenSpaceIdWriteMaterial;
		public Texture2D ScreenSpaceUnitIdTexture;

		private int UnitIndex = 0;

		public Unit UnitUnderScreenPoint(Vector2 screenPoint)
		{
			if (Input.mousePosition.x >= 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y >= 0 && Input.mousePosition.y < Screen.height) 
			{
				
				RenderTexture.active = ScreenSpaceUnitIdRenderTexture;

				// this line might bug out on differant platforms
				// see this page http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
				// if it does, there are some shader tricks we can do to fix it
				ScreenSpaceUnitIdTexture.ReadPixels (new Rect (Input.mousePosition.x, Screen.height - Input.mousePosition.y, 1, 1), 0, 0, false);
				ScreenSpaceUnitIdTexture.Apply ();

				RenderTexture.active = null;
				
				UnitIndex = (int)ScreenSpaceUnitIdTexture.GetPixel (0, 0).r;
			}
			else
			{
				UnitIndex = 0;
			}

			if (Unit.unitLookup.ContainsKey (UnitIndex)) 
			{
				return Unit.unitLookup [UnitIndex];
			} 
			else 
			{
				return null;
			}
		}

		protected override void Start ()
		{
			base.Start ();
			screenSpaceIdWrite = Shader.Find ("ImageEffect/ScreenSpaceWriteId");
			screenSpaceIdWriteMaterial = new Material (screenSpaceIdWrite);
			ScreenSpaceUnitIdTexture = new Texture2D (1, 1, TextureFormat.RGBAFloat,false);
			//ScreenSpaceUnitIdRenderTexture = RenderTexture.GetTemporary ((int)(Screen.width / 2.0f), (int)(Screen.height / 2.0f), 24,RenderTextureFormat.ARGBFloat);
		}

		private void OnPostRender()
		{
			if (ScreenSpaceUnitIdRenderTexture != null)
				RenderTexture.ReleaseTemporary (ScreenSpaceUnitIdRenderTexture);

			ScreenSpaceUnitIdRenderTexture = RenderTexture.GetTemporary (Screen.width, Screen.height, 24,RenderTextureFormat.ARGBFloat);
			//ScreenSpaceUnitIdTexture.Resize (Screen.width, Screen.height, TextureFormat.RGBAFloat, false);
			//ScreenSpaceUnitIdTexture.Apply ();

			// screen space unit id's
			// clear the render texture
			RenderTexture.active = ScreenSpaceUnitIdRenderTexture;
			UnityEngine.GL.Clear (true, true, Color.clear);
			RenderTexture.active = null;

			//set up graphics commands to render units
			CommandBuffer buf = new CommandBuffer ();
			foreach (Unit unit in Unit.unitLookup.Select(x => x.Value)) 
			{
				buf.SetGlobalFloat ("id", unit.id);
				foreach(Renderer renderer in unit.GetComponentsInChildren<Renderer>())
				{
					buf.DrawRenderer(renderer,screenSpaceIdWriteMaterial);
				}
			}

			// render unit ids to rendertexture
			RenderTexture.active = ScreenSpaceUnitIdRenderTexture;
			Graphics.ExecuteCommandBuffer (buf);
			RenderTexture.active = null;

			//RenderTexture.ReleaseTemporary (ScreenSpaceUnitIdRenderTexture);
		}
	}
}