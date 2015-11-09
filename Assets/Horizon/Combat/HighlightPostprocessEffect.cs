using UnityEngine;
using System.Collections;


//TODO refactor this so that you dont have to have two cameras in the scene, like how i do it in combat camera
//in fact, this should probably be moved to combat camera
public class HighlightPostprocessEffect : MonoBehaviour {

	//camera which ojnly renders a layer with what we want to highlight on it
	public Camera HighlightCamera;

	public int outlinePixleThickness = 1;

	private void clearRenderTexture(RenderTexture tex)
	{
		RenderTexture.active = tex;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = null;
	}

	public void Start()
	{
		alphaEdge = new Material (Shader.Find ("ImageEffect/AlphaEdge"));
		applyOutline = new Material (Shader.Find ("ImageEffect/ApplyOutline"));
		overlay = new Material (Shader.Find ("ImageEffect/Overlay"));
	}

	void OnPostRender()
	{
		//render the objcts to highlight into a texture
		RenderTexture highlightedObjects = RenderTexture.GetTemporary(Screen.width, Screen.height);
		clearRenderTexture (highlightedObjects);
		HighlightCamera.targetTexture = highlightedObjects;
		HighlightCamera.Render ();

		//store copy of highlightedObject textures, and succsesive layers of the generated edge
		RenderTexture cumulativeEdge = RenderTexture.GetTemporary(Screen.width, Screen.height);
		clearRenderTexture (cumulativeEdge);
		Graphics.Blit (highlightedObjects, cumulativeEdge, overlay);

		//texture that holds the result (edge of single pixle width) of running the aplha edge shader on cumulativeEdge
		RenderTexture EdgeTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
		clearRenderTexture (EdgeTexture);

		//step from one pixle to the next
		Shader.SetGlobalVector ("stepSize", new Vector4 (1.0f / (float)Screen.width, 1.0f / (float)Screen.height, 0, 0));

		//run edgedetection multiple times to generat outline of correct thinkness
		for (int i = 0; i < outlinePixleThickness; i++) 
		{
			//draw single pixle edge of highlited objects to
			Graphics.Blit (cumulativeEdge, EdgeTexture, alphaEdge);
			//copy edge back to cumulativeEdge
			Graphics.Blit (EdgeTexture, cumulativeEdge, overlay);
			//clear edge texture
			clearRenderTexture (EdgeTexture);
		}

		//apply outline to screen
		Shader.SetGlobalTexture ("originalObjects", highlightedObjects);
		Shader.SetGlobalColor ("outlineColor",Color.cyan);
		Graphics.Blit (cumulativeEdge, null, applyOutline);

		RenderTexture.ReleaseTemporary (highlightedObjects);
		RenderTexture.ReleaseTemporary (cumulativeEdge);
		RenderTexture.ReleaseTemporary (EdgeTexture);
	}

	private Material alphaEdge;
	private Material applyOutline;
	private Material overlay;
}
