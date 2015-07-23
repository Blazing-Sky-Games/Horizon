using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Gamelogic.Grids;

// displays grid lines in game
// handles user input related to the grid hmm... this should be factored out into a specific class for it.
// also ... is how you edit the grid size in the scene view ... huh? shouldnt this be driven by the model ... ugh
// we really need to write our own grid code one of these days
public class HorizonGridGameView : RectTileGridBuilder
{
	// which point is under the mouse
	private RectPoint PointUnderMouse;
	// ray from the camera that goes where the mouse is
	private Ray mouseRay;

	// stack of highligh sets
	// i think there is a better way to do highlighting that this stack based approche
	private Stack<IEnumerable<RectPoint>> highlighSets = new Stack<IEnumerable<RectPoint>>();

	[SerializeField] // wait why is this serialized
	public Material lineMaterial; // material of grid lines

	// this grid model
	public HorizonGridModel model;

	// are we proccesing clicks
	public bool handleInput;

	// highlight a set of cells
	public void pushHighlightSet(IEnumerable<RectPoint> points, Color color)
	{
		if(points == null) return;

		foreach(RectPoint point in points)
		{
			model.CellViewGrid[point].pushHighlightColor(color);
		}

		highlighSets.Push(points);
	}

	// remove the top most highlight set
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
		// huh? why the hell do i do this
		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent -= OnPostRender;
		
		updateType = UpdateType.EditorAuto; // dumb thing i set for Gamelogic.Grids
		plane = MapPlane.XZ; // dumb thing i set for Gamelogic.Grids
		Alignment = MapAlignment.BottomLeft; // dumb thing i set for Gamelogic.Grids
		cellSpacingFactor = new Vector2(1,1); // dumb thing i set for Gamelogic.Grids

		// oh god now i remember how hacky this file is
		// creat a new grid
		base.Grid = RectGrid<TileCell>.Rectangle(Dimensions.X, Dimensions.Y);

		// get the model
		model = gameObject.GetComponent<HorizonGridModel>();

		// handle input
		handleInput = true;

		// this event tells us to draw the grid lines
		// gl drawing can only be done during post render, not during regualr update
		// hence why we need this event
		// hmm... i wonder if we could draw cells using gl instead of using sprites ... if we ever wnated more complicated cells we couldnt... or could we?
		Camera.main.GetComponent<SimpleCameraControls>().PostRenderEvent += OnPostRender;
	}

	// called every frame
	// the new keyword makes this overide the update function from the parent class
	// its a weird c# thing just pretent it isnt there
	new void Update()
	{
		UpdatePointUnderMouse();

		if(handleInput)
		{
			ProcessInput();
		}
	}

	// do all of our gl drawing
	void OnPostRender()
	{
		DrawGridLines();
	}

	//draw grid lines around all passable cells
	void DrawGridLines ()
	{
		// init boilerplate
		lineMaterial.SetPass( 0 );
		GL.Begin( GL.LINES );
		GL.Color(Color.black * new Color(1,1,1,0.8f));

		//goes step by step drawaing cell width line segments
		for(int j = 0; j <= Dimensions.Y; j += 1)
		{
			for(int i = 0; i < Dimensions.X; i += 1)
			{
				bool draw = true;
				if(j == 0)
				{
					//we are on the edge, only check one cell
					draw = model.CellViewGrid[new RectPoint(i,j)].model.state == CellState.Passable;
				}
				else if(j == Dimensions.Y)
				{
					//we are on the edge, only check one cell
					draw = model.CellViewGrid[new RectPoint(i,j - 1)].model.state == CellState.Passable;
				}
				else
				{
					// draw this line if either of the bordering cells are passable
					CellState forwardState = model.CellViewGrid[new RectPoint(i,j)].model.state;
					CellState backState = model.CellViewGrid[new RectPoint(i, j-1)].model.state;
					draw = (backState == CellState.Passable || forwardState == CellState.Passable);
				}

				// draw the line segment
				if(draw)
				{
					GL.Vertex3( transform.position.x + i * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + j * CellSpacingFactor.y );
					GL.Vertex3( transform.position.x + (i+1) * CellSpacingFactor.x, transform.position.y + 0.01f, transform.position.z + j * CellSpacingFactor.y );
				}
			}
		}

		//like the loop above, but goind in the other direction
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

	// do some raycating and stuff to figure out what grid point the mouse is over
	void UpdatePointUnderMouse()
	{
		// this vector is (mousepos.x,mousepos.y, distance from camera to near cliping plane)
		Vector3 CorrectedMousePosision = Input.mousePosition;
		CorrectedMousePosision.z = Camera.main.nearClipPlane;

		// get a ray going through the mouse point on the near clippping plane
		mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);
		
		// a plane in line with the grid
		Plane gridPlane = new Plane(new Vector3(0,1,0),transform.position);
		
		// get the percentage along the ray that it intersect the plane
		float RayPlaneIntersectionAt;
		gridPlane.Raycast(mouseRay, out RayPlaneIntersectionAt);
		
		// get the posision in space that the ray intersects the plane
		Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * RayPlaneIntersectionAt - transform.position;
		
		// convert 3d posision to grid point
		int a = (int)Mathf.Floor(planeIntersect.x / CellSpacingFactor.x);
		int b = (int)Mathf.Floor(planeIntersect.z / CellSpacingFactor.y);
		PointUnderMouse = new RectPoint(a, b);

		RaycastHit hit;
		if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true)
		{
			// if we are over a ui element dont highlight a unit
			model.HighlightedUnit = null;
		}
		else if(Physics.Raycast(mouseRay,out hit,100))
		{
			// if we are over a unit, make it the highlighted unit
			model.HighlightedUnit = hit.collider.GetComponentInParent<HorizonUnitModel>();
		}
		else
		{
			// the mouse is not over a unit
			model.HighlightedUnit = null;
		}

		if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true)
		{
			// if we are over a ui element dont highlight a cell
			model.HighlightedCell = null;
		}
		else if (Grid.Contains(PointUnderMouse) && model.HighlightedUnit == null)
		{
			// if we are over a cell, make it the highlighted cell
			model.HighlightedCell = model.CellViewGrid[PointUnderMouse];
		}
		else
		{
			// the mouse is not over a cell
			model.HighlightedCell = null;
		}
	}
	
	//handle mouse clicks
	private void ProcessInput()
	{
		// if the user clicks while not over a ui element
		if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
		{
			RaycastHit hit;
			if(Physics.Raycast(mouseRay,out hit,100))
			{
				HorizonUnitModel unitModel = hit.collider.GetComponentInParent<HorizonUnitModel>();
				if(unitModel != null) model.SelectedUnit = unitModel; // if we hit a unit, that unit is selected
			}
			else if (Grid.Contains(PointUnderMouse))
			{
				model.SelectedCell = model.CellViewGrid[PointUnderMouse]; // if we did not hit a unit, and the mouse point is in the grid, select that cell
			}
		}
	}

	// ?????? i dont think i even use this
	private void SendMessageToGridAndCell(RectPoint point, string message)
	{
		SendMessage(message, point, SendMessageOptions.DontRequireReceiver);
		
		if (Grid[point] != null)
		{
			Grid[point].SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}
}
