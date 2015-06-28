using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Gamelogic.Grids;

public class HorizonGridView : RectTileGridBuilder
{
	private RectPoint PointUnderMouse;
	private Ray mouseRay;

	private Stack<IEnumerable<RectPoint>> highlighSets = new Stack<IEnumerable<RectPoint>>();

	[SerializeField]
	public Material lineMaterial;

	public HorizonGridModel model;

	public bool handleInput;

	public void pushHighlightSet(IEnumerable<RectPoint> points, Color color)
	{
		if(points == null) return;

		foreach(RectPoint point in points)
		{
			model.CellViewGrid[point].pushHighlightColor(color);
		}

		highlighSets.Push(points);
	}

	public void popHighlightSet()
	{
		if(highlighSets.Count == 0) return;

		foreach(RectPoint point in highlighSets.Peek())
		{
			model.CellViewGrid[point].popHighlightColor();
		}

		highlighSets.Pop();
	}

	protected override void InitGrid ()
	{
		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent -= OnPostRender;
		
		updateType = UpdateType.EditorAuto;
		plane = MapPlane.XZ;
		Alignment = MapAlignment.BottomLeft;
		cellSpacingFactor = new Vector2(1,1);

		base.Grid = RectGrid<TileCell>.Rectangle(Dimensions.X, Dimensions.Y);

		model = gameObject.GetComponent<HorizonGridModel>();

		handleInput = true;

		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent += OnPostRender;
	}

	new void Update()
	{
		UpdatePointUnderMouse();

		if(handleInput)
		{
			ProcessInput();
		}
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

		RaycastHit hit;
		if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true)
		{
			model.HighlightedUnit = null;
		}
		else if(Physics.Raycast(mouseRay,out hit,100))
		{
			model.HighlightedUnit = hit.collider.GetComponentInParent<HorizonUnitModel>();
		}
		else
		{
			model.HighlightedUnit = null;
		}

		if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true)
		{
			model.HighlightedCell = null;
		}
		else if (Grid.Contains(PointUnderMouse) && model.HighlightedUnit == null)
		{
			model.HighlightedCell = model.CellViewGrid[PointUnderMouse];
		}
		else
		{
			model.HighlightedCell = null;
		}
	}
	
	private void ProcessInput()
	{
		if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
		{
			RaycastHit hit;
			if(Physics.Raycast(mouseRay,out hit,100))
			{
				HorizonUnitModel unitModel = hit.collider.GetComponentInParent<HorizonUnitModel>();
				if(unitModel != null) model.SelectedUnit = unitModel;
			}
			else if (Grid.Contains(PointUnderMouse))
			{
				model.SelectedCell = model.CellViewGrid[PointUnderMouse];
			}
		}
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
