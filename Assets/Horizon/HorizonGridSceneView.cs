using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using Gamelogic.Grids;

public class HorizonGridSceneView : MonoBehaviour 
{
	[SerializeField]
	int selectedCells = 0;

	[SerializeField]
	HorizonCellView lastSelectedCell = null;

	[SerializeField]
	Tool lastTool = Tool.None;

	[SerializeField]
	HorizonGridView view = null;

	void IterateRectPointRange(Vector2 start, Vector2 end, Action<Transform> action)
	{
		float minX = Mathf.Min(start.x,end.x);
		float maxX = Mathf.Max(start.x,end.x);
		float minY = Mathf.Min(start.y,end.y);
		float maxY = Mathf.Max(start.y,end.y);
		
		for(int i = (int)Mathf.Ceil(minX-1); i <= (int)Mathf.Floor(maxX); i++)
		{
			for(int j = (int)Mathf.Ceil(minY-1); j <= (int)Mathf.Floor(maxY); j++)
			{
				HorizonCellView cell = (HorizonCellView)view.Grid[new RectPoint(i,j)];
				if(cell != null) action(cell.transform);
			}
		}
	}

	void OnDrawGizmos()
	{
		if(view == null) view = gameObject.GetComponent<HorizonGridView>();

		// Shift select for cells
		int cellCount = Selection.gameObjects.Count(x => x.GetComponent<HorizonCellView>() != null);
		if(cellCount != 0)
		{
			lastTool = Tools.current == Tool.None ? lastTool : Tools.current;
			Tools.current = Tool.None;
			
			if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<HorizonCellView>() != null)
			{
				if(cellCount > selectedCells)
				{
					selectedCells = cellCount;
					if(Event.current.shift)
					{
						if(lastSelectedCell == null)
						{
							lastSelectedCell = Selection.activeGameObject.GetComponent<HorizonCellView>();
						}
						else
						{
							HorizonCellView currentCell = Selection.activeGameObject.GetComponent<HorizonCellView>();
							
							Vector3 center = currentCell.transform.position - transform.position;
							float a = (center.x / view.CellSpacingFactor.x);
							float b = (center.z / view.CellSpacingFactor.y);
							Vector2 currentPoint = new Vector2(a,b);
							
							center = lastSelectedCell.transform.position - transform.position;
							a = (center.x / view.CellSpacingFactor.x);
							b = (center.z / view.CellSpacingFactor.y);
							Vector2 lastPoint = new Vector2(a,b);
							
							List<GameObject> pointsInRange = new List<GameObject>();

							IterateRectPointRange(lastPoint, currentPoint, (cell) => 
							{
								if(pointsInRange.Contains(cell.gameObject) == false) 
								pointsInRange.Add(cell.gameObject);
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
						lastSelectedCell = Selection.activeGameObject.GetComponent<HorizonCellView>();
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


		// grid lines in scene view
		Handles.color = Color.cyan;
		Handles.color = Handles.color*new Color(1,1,1,0.75f);

		for(int i = 0; i <= view.Dimensions.X; i++)
		{
			Vector3 point = transform.position + new Vector3(i*view.CellSpacingFactor.x,0,0) + Vector3.up*0.01f;
			Handles.DrawDottedLine(point, point + Vector3.forward*view.CellSpacingFactor.y*view.Dimensions.Y,4);
		}
		
		for(int i = 0; i <= view.Dimensions.Y; i++)
		{
			Vector3 point = transform.position + new Vector3(0,0,i*view.CellSpacingFactor.y) + Vector3.up*0.01f;
			Handles.DrawDottedLine(point, point + Vector3.right*view.CellSpacingFactor.x*view.Dimensions.X,4);
		}
	}

	void OnDrawGizmosSelected()
	{

	}
}
