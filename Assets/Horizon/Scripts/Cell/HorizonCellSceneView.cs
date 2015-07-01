// this is an editor script, so only use this if we are in the editor
#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;
using Gamelogic.Grids;

// controls scene view highlighting while editing cells
public class HorizonCellSceneView : MonoBehaviour 
{
	//i really need to look into if i am using [SerializeField] right
	[SerializeField]
	private HorizonCellModel model = null;

	[SerializeField]
	private HorizonCellGameView view = null;

	[SerializeField]
	// what color is this in the scene view
	private Color sceneViewHighlight;

	[SerializeField]
	// mesh used to render cell highlights
	private Mesh RectangleMesh;


	//hmm scripts like this run in the editor so they dont have start/awake or other ini functions like that. 
	//hmm... maybe we could use OnEnable to do init

	// helper function to draw a rectangle in the scene view
	void DrawSolidRectangleGizmo(Color color)
	{
		// if this is the first time we are using this
		// creat the rectangle mesh
		if(RectangleMesh == null)
		{
			//todo: make this mesh the right size even if the cell size changes
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

		// gizmos are stupid and you have to set the global gizmo drawing color befor you call a gizmo draw function
		// in order to control the color
		Gizmos.color = color;
		Gizmos.DrawMesh(RectangleMesh);
	}

	void UpdateSceneViewHighlight()
	{
		// change this code to change how cells behave visually in the editor
		// hmm ... make these colors visible in a "globals" singleton script
		// hmm ... is there a better way to do this
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

	// called every frame in the scene view
	public void OnDrawGizmos()
	{
		// init variables, because we dont have an init type function yet
		if(model == null) model = gameObject.GetComponent<HorizonCellModel>();
		if(view == null) view  = gameObject.GetComponent<HorizonCellGameView>();

		UpdateSceneViewHighlight();

		DrawSolidRectangleGizmo(sceneViewHighlight);
	}
	
	// called every frame in the scene view that this game object is selected
	void OnDrawGizmosSelected()
	{
		// draw a little box with the name (which is always the coordinates of) the cell
		GLGizmos.Label(transform.position, name);
	}
}
#endif
