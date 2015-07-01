#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Internal;
using UnityEditor;

using System.Collections;
using System.Linq;

using Gamelogic.Grids;

public class HorizonCellSceneView : MonoBehaviour 
{
	[SerializeField]
	private HorizonCellModel model = null;

	[SerializeField]
	private HorizonCellGameView view = null;

	[SerializeField]
	private Color sceneViewHighlight;

	[SerializeField]
	private Mesh RectangleMesh;

	void DrawSolidRectangleGizmo(Color color)
	{
		if(RectangleMesh == null)
		{
			RectangleMesh = new Mesh();
			RectangleMesh.vertices = new Vector3[]{ 
				transform.position + new Vector3(0.5f,0.01f,0.5f), 
				transform.position + new Vector3(-0.5f,0.01f,0.5f), 
				transform.position + new Vector3(-0.5f,0.01f,-0.5f), 
				transform.position + new Vector3(0.5f,0.01f,-0.5f)
			};
			
			RectangleMesh.triangles = new int[]{
				2,1,0,
				3,2,0
			};
			
			RectangleMesh.normals = new Vector3[]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
		}

		Gizmos.color = color;
		Gizmos.DrawMesh(RectangleMesh);
	}

	void UpdateSceneViewHighlight()
	{
		switch(model.state)
		{
		case CellState.Passable:
			sceneViewHighlight = Color.cyan * new Color(0.8f,0.8f,0.8f,0.1f);
			break;
		case CellState.NonPassable:
			sceneViewHighlight = Color.black * new Color(0.8f,0.8f,0.8f,0.5f);
			break;
		default:
			sceneViewHighlight = Color.cyan * new Color(0.8f,0.8f,0.8f,0.1f);
			break;
		}
	}

	public void OnDrawGizmos()
	{
		if(model == null) model = gameObject.GetComponent<HorizonCellModel>();
		if(view == null) view  = gameObject.GetComponent<HorizonCellGameView>();

		UpdateSceneViewHighlight();

		DrawSolidRectangleGizmo(sceneViewHighlight);
	}
	
	void OnDrawGizmosSelected()
	{
		// draw in scene view when selected
		GLGizmos.Label(transform.position, name);
	}
}
#endif
