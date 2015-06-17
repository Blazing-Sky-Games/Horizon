using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Gamelogic.Grids;
using System.Linq;
using UnityEditor;


public static class vectorPointExtentions
{
    public static Vector2 toVec2(this VectorPoint self)
    {
        return new Vector2(self.X, self.Y);
    }
}

public class select3d : GridBehaviour<RectPoint>
{
	[SerializeField]
	private Tool lastTool = Tool.None;

	[SerializeField]
	public static bool shift = false;

	[SerializeField]
	private Mockupcell lastSelectedCell = null;

	[SerializeField]
	public int selectedCells = 0;

    public static select3d instance;

	[SerializeField]
	public int width = 10;
	[SerializeField]
	public float spacing = 1;
	[SerializeField]
	public MapAlignment align = MapAlignment.MiddleCenter;

    void Awake()
    {
        instance = this;
    }

    public CharacterControler character;

    private IGrid<Mockupcell, RectPoint> playgrid;

    private Vector2 dimensions;
    private Vector2 cellFactor;
    private Plane gridPlane;

    public override void InitGrid()
    {
        playgrid = Grid.CastValues<Mockupcell, RectPoint>();

        dimensions = GridBuilder.Dimensions.toVec2();
        cellFactor = GridBuilder.CellSpacingFactor;

        Vector3 gridNormal = GridBuilder.Plane == MapPlane.XY ? Vector3.forward: Vector3.up;

        gridPlane = new Plane(gridNormal,transform.position);

        character.init();
    }

    void Update()
    {
        Vector3 CorrectedMousePosision = Input.mousePosition;
        CorrectedMousePosision.z = Camera.main.nearClipPlane;

        Ray mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);

        float enter;
        bool parralel = gridPlane.Raycast(mouseRay, out enter);

        Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * enter;

        int a = (int)Mathf.Ceil(planeIntersect.x / cellFactor.x);
        int b = (int)Mathf.Ceil(planeIntersect.z / cellFactor.y);

        if (GridBuilder.Plane == MapPlane.XY)
        {
            b = (int)Mathf.Ceil(planeIntersect.y / cellFactor.y);
        }

        RectPoint mousepoint = new RectPoint(a, b) + new RectPoint(4, 4);

        if(Input.GetMouseButtonDown(0) && !playgrid.IsOutside(mousepoint))
        {
            MyOnClick(mousepoint);
        }

        if (character.isMoving == false)
            SetHover(mousepoint);
        else
            playgrid[hover].highlighted = false;  
    }

    void MyOnClick(RectPoint point)
    {
        if (character.isMoving == false && playgrid[point].passable)
        {
            character.setDest(point);
        }
    }

    private void SetHover(RectPoint currenthover)
    {
        if (playgrid.IsOutside(currenthover))
        {
            playgrid[hover].highlighted = false;
            wasoutside = true;
            return;
        }

        if (wasoutside)
        {
            playgrid[currenthover].highlighted = true;
            hover = currenthover;
            wasoutside = false;
            return;
        }

        if (currenthover != hover)
        {
            playgrid[currenthover].highlighted = true;
            playgrid[hover].highlighted = false;
            hover = currenthover;
        }
    }
    private RectPoint hover;
    private bool wasoutside = false;

	Transform getCellFromRectPointInEditor(RectPoint Point)
	{
		string name = "(";
		name += Point.X;
		name += ", ";
		name += Point.Y;
		name += ")";

		return transform.FindChild(name);
	}

	void IterateRectPointRange(RectPoint start, RectPoint end, Action<Transform> action)
	{
		int minX = Math.Min(start.X,end.X);
		int maxX = Math.Max(start.X,end.X);
		int minY = Math.Min(start.Y,end.Y);
		int maxY = Math.Max(start.Y,end.Y);

		for(int i = minX; i <= maxX; i++)
		{
			for(int j = minY; j <= maxY; j++)
			{
				Transform cell = getCellFromRectPointInEditor(new RectPoint(i,j));
				if(cell != null) action(cell);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		int cellCount = Selection.gameObjects.Count(x => x.GetComponent<Mockupcell>() != null);
		if(cellCount != 0)
		{
			lastTool = Tools.current == Tool.None ? lastTool : Tools.current;
			Tools.current = Tool.None;

			if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Mockupcell>() != null)
			{
				if(cellCount > selectedCells)
				{
					selectedCells = cellCount;
					if(shift)
					{
						if(lastSelectedCell == null)
						{
							lastSelectedCell = Selection.activeGameObject.GetComponent<Mockupcell>();
						}
						else
						{
							Mockupcell currentCell = Selection.activeGameObject.GetComponent<Mockupcell>();

							Vector3 pos = currentCell.Center;

							int a = (int)Mathf.Ceil(pos.x / spacing);
							int b = (int)Mathf.Ceil(pos.z / spacing);
							
							if (GridBuilder.Plane == MapPlane.XY)
							{
								b = (int)Mathf.Ceil(pos.y / spacing);
							}

							RectPoint current = new RectPoint(a,b) + new RectPoint(4, 4);

							pos = lastSelectedCell.Center;
							
							a = (int)Mathf.Ceil(pos.x / spacing);
							b = (int)Mathf.Ceil(pos.z / spacing);
							
							if (GridBuilder.Plane == MapPlane.XY)
							{
								b = (int)Mathf.Ceil(pos.y / spacing);
							}

							RectPoint last = new RectPoint(a,b) + new RectPoint(4, 4);

							List<GameObject> pointsInRange = new List<GameObject>();

							IterateRectPointRange(last,current, (cell) =>
							{
								if(pointsInRange.Contains(cell.gameObject) == false) pointsInRange.Add(cell.gameObject);
							});

							foreach(GameObject obj in Selection.gameObjects)
							{
								if(pointsInRange.Contains(obj) == false) pointsInRange.Add(obj);
							}

							Selection.objects = pointsInRange.ToArray();

							lastSelectedCell = currentCell;
						}
					}
					else
					{
						lastSelectedCell = Selection.activeGameObject.GetComponent<Mockupcell>();
					}
				}
			}
		}
		else
		{
			lastSelectedCell = null;
			selectedCells = 0;
			Tools.current = lastTool == Tool.None ? Tools.current : lastTool;
			lastTool = Tool.None;
		}

		Handles.color = Color.cyan;
		Handles.color = Handles.color*new Color(1,1,1,0.75f);

		for(int i = 0; i < width; i++)
		{
			Vector3 pointX = transform.position + new Vector3(i*spacing,0,0) + new Vector3(-spacing*width / 2, 0, -spacing*width / 2);
			Handles.DrawDottedLine(pointX,pointX + Vector3.forward*spacing*width,4);

			Vector3 pointZ = transform.position + new Vector3(0,0,i*spacing) + new Vector3(-spacing*width / 2, 0, -spacing*width / 2);
			Handles.DrawDottedLine(pointZ,pointZ + Vector3.right*spacing*width,4);
		}
	}
}
