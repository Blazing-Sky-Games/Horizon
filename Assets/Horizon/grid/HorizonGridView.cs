using UnityEngine;
using UnityEditor;

using System.Collections;
using System;

using Gamelogic.Grids;

[ExecuteInEditMode]
public class HorizonGridView : RectTileGridBuilder
{
	private RectPoint PointUnderMouse;
	private Ray mouseRay;

	[SerializeField]
	public Material lineMaterial;

	public HorizonGridModel model;

	protected override void InitGrid ()
	{
		//called whenever the grid gets reset
		//NEED TO MAKE SURE CELL PROPS STAY

		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent -= OnPostRender;
		
		updateType = UpdateType.EditorAuto;
		plane = MapPlane.XZ;
		Alignment = MapAlignment.BottomLeft;
		cellSpacingFactor = new Vector2(1,1);

		base.Grid = RectGrid<TileCell>.Rectangle(Dimensions.X, Dimensions.Y);

		model = gameObject.GetComponent<HorizonGridModel>();

		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent += OnPostRender;
	}

	new void Update()
	{
		UpdatePointUnderMouse();

		ProcessInput();
	}

	void OnPostRender()
	{
		DrawGridLines();
	}

	void DrawGridLines ()
	{
		lineMaterial.SetPass( 0 );

		GL.Begin( GL.LINES );

		GL.Color(Color.black * new Color(1,1,1,0.8f));

		for(int j = 0; j <= Dimensions.Y; j += 1)
		{
			for(int i = 0; i < Dimensions.X; i += 1)
			{
				bool draw = true;
				if(j == 0)
				{
					draw = model.CellViewGrid[new RectPoint(i,j)].model.state == CellState.Passable;
				}
				else if(j == Dimensions.Y)
				{
					draw = model.CellViewGrid[new RectPoint(i,j - 1)].model.state == CellState.Passable;
				}
				else
				{
					CellState forwardState = model.CellViewGrid[new RectPoint(i,j)].model.state;
					CellState backState = model.CellViewGrid[new RectPoint(i, j-1)].model.state;
					draw = (backState == CellState.Passable || forwardState == CellState.Passable);
				}

				if(draw)
				{
					GL.Vertex3( transform.position.x + i * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + j * CellSpacingFactor.y );
					GL.Vertex3( transform.position.x + (i+1) * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + j * CellSpacingFactor.y );
				}
			}
		}

		for(int i = 0; i <= Dimensions.X; i += 1)
		{
			for(int j = 0; j < Dimensions.Y; j += 1)
			{
				bool draw = true;
				if(i == 0)
				{
					draw = model.CellViewGrid[new RectPoint(i,j)].model.state == CellState.Passable;
				}
				else if(i == Dimensions.X)
				{
					draw = model.CellViewGrid[new RectPoint(i-1,j)].model.state == CellState.Passable;
				}
				else
				{
					CellState forwardState = model.CellViewGrid[new RectPoint(i,j)].model.state;
					CellState backState = model.CellViewGrid[new RectPoint(i-1, j)].model.state;
					draw = (backState == CellState.Passable || forwardState == CellState.Passable);
				}
				
				if(draw)
				{
					GL.Vertex3( transform.position.x + i * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + j * CellSpacingFactor.y );
					GL.Vertex3( transform.position.x + i * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + (j+1) * CellSpacingFactor.y );
				}
			}
		}

		//for(float i = 0; i <= Dimensions.X * CellSpacingFactor.x; i += CellSpacingFactor.x)
		//{
			//GL.Vertex3( transform.position.x + i, transform.position.y + 0.01f, transform.position.z );
			//GL.Vertex3( transform.position.x + i, transform.position.y + 0.01f, transform.position.z + Dimensions.Y * CellSpacingFactor.y);
		//}

		GL.End();
	}

	void UpdatePointUnderMouse()
	{
		Vector3 CorrectedMousePosision = Input.mousePosition;
		CorrectedMousePosision.z = Camera.main.nearClipPlane;

		mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);
		
		Plane gridPlane = new Plane(new Vector3(0,1,0),transform.position);
		
		float RayPlaneIntersectionAt;
		gridPlane.Raycast(mouseRay, out RayPlaneIntersectionAt);
		
		Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * RayPlaneIntersectionAt - transform.position;
		
		int a = (int)Mathf.Floor(planeIntersect.x / CellSpacingFactor.x);
		int b = (int)Mathf.Floor(planeIntersect.z / CellSpacingFactor.y);
		
		PointUnderMouse = new RectPoint(a, b);
	}
	
	private void ProcessInput()
	{
		if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
		{
			RaycastHit hit;
			if(Physics.Raycast(mouseRay,out hit,100,LayerMask.NameToLayer("units")))
			{
				//select unit
			}
			else if (Grid.Contains(PointUnderMouse))
			{
				//SendMessageToGridAndCell(PointUnderMouse, "OnClick");
				//print(PointUnderMouse);
				//select cell
			}
		}

		//other input processing
	}

	private void SendMessageToGridAndCell(RectPoint point, string message)
	{
		SendMessage(message, point, SendMessageOptions.DontRequireReceiver);
		
		if (Grid[point] != null)
		{
			Grid[point].SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}
}
