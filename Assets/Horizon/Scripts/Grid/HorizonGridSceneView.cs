// this is an editor script, so only include it if we are running in the editor
// hmm ... i wonder if we could get around this by just sticking it in the "editor" folder
//research that later
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Gamelogic.Grids;

// does two things right now. 
//draws grid lines in the scene view, 
//and enables cool shift select feature for cells in the scene view
//lol, i probably didnt have to go through the trouble to make this
public class HorizonGridSceneView : MonoBehaviour 
{
	// these fields need to be serialized to be used in the gizmo stuff
	[SerializeField]
	//the number of cells currently selected
	int selectedCells = 0;

	[SerializeField]
	HorizonCellGameView lastSelectedCell = null;

	[SerializeField]
	// the last tool that was used in the scene view
	Tool lastTool = Tool.None;

	[SerializeField]
	HorizonGridGameView view = null;

	// helper function to iterate through a set of cells gives the min and max points of a rectangle
	void IterateRectPointRange(Vector2 start, Vector2 end, Action<Transform> action)
	{
		float minX = Mathf.Min(start.x,end.x);
		float maxX = Mathf.Max(start.x,end.x);
		float minY = Mathf.Min(start.y,end.y);
		float maxY = Mathf.Max(start.y,end.y);
		
		// not sure about the ceil and floor stuff ... i just played around with it until it worked
		for(int i = (int)Mathf.Ceil(minX-1); i <= (int)Mathf.Floor(maxX); i++)
		{
			for(int j = (int)Mathf.Ceil(minY-1); j <= (int)Mathf.Floor(maxY); j++)
			{
				HorizonCellGameView cell = (HorizonCellGameView)view.Grid[new RectPoint(i,j)];
				if(cell != null) action(cell.transform);
			}
		}
	}

	//called everty frame in the scene view
	void OnDrawGizmos()
	{
		// init stuff, because we dont have a init typefunction in sceneviews
		if(view == null) view = gameObject.GetComponent<HorizonGridGameView>();

		// implement shift select in the scene view
		// when the user clicks a cell, holds shift, and clicks another cell
		// all the cells in a rectragle defined by those two cells are also selected
		// hmm ... this is alittle buggy right now

		//how many cells are selected right now
		int cellCount = Selection.gameObjects.Count(x => x.GetComponent<HorizonCellGameView>() != null);
		if(cellCount != 0)
		{
			//save the last tool that was active in the scene view.
			//we dont want the user to be able to move cells around (it would mess up the grid)
			//so set the current scene view tool to none
			lastTool = Tools.current == Tool.None ? lastTool : Tools.current;
			Tools.current = Tool.None;
			
			// if the last selected object is a cell
			if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<HorizonCellGameView>() != null)
			{
				//if we have selected a new cell (if the number of)
				if(cellCount > selectedCells)
				{
					// the number of cells selected has changed
					selectedCells = cellCount;
					//if the user is holding down shift
					if(Event.current.shift)
					{
						//if this is the first cell in the shift select
						if(lastSelectedCell == null)
						{
							// set the last selected cell to the currently selected cell
							lastSelectedCell = Selection.activeGameObject.GetComponent<HorizonCellGameView>();
						}
						//if this is the second cell selected with shift select
						else
						{
							HorizonCellGameView currentCell = Selection.activeGameObject.GetComponent<HorizonCellGameView>();
							
							// compute the rect point of the current cell ... wait ... coulnt we just use model.positionpoint?
							Vector3 center = currentCell.transform.position - transform.position;
							float a = (center.x / view.CellSpacingFactor.x);
							float b = (center.z / view.CellSpacingFactor.y);
							Vector2 currentPoint = new Vector2(a,b);
							
							// compute the rect point of the last cell
							center = lastSelectedCell.transform.position - transform.position;
							a = (center.x / view.CellSpacingFactor.x);
							b = (center.z / view.CellSpacingFactor.y);
							Vector2 lastPoint = new Vector2(a,b);
							
							List<GameObject> pointsInRange = new List<GameObject>();

							// get all of the points in the rectangle
							IterateRectPointRange(lastPoint, currentPoint, (cell) => 
							{
								if(pointsInRange.Contains(cell.gameObject) == false) 
								pointsInRange.Add(cell.gameObject);
							});
							
							// include the already selected objects
							foreach(GameObject obj in Selection.gameObjects)
							{
								if(pointsInRange.Contains(obj) == false) pointsInRange.Add(obj);
							}
							
							// set the selection
							Selection.objects = pointsInRange.ToArray();
							
							// hmm .... should this be lastSelectedCell = null?
							lastSelectedCell = currentCell;
						}
					}
					else
					{
						//if shift is not held down, always capture the selected cell as the start of a shift select
						lastSelectedCell = Selection.activeGameObject.GetComponent<HorizonCellGameView>();
					}
				}
			}
		}
		// if the last selected object is not a cell
		else
		{
			// ummm ... i know i do this for some reason ... not sure why
			// it is probs a bad sign that i dont know what these two lines are for
			lastSelectedCell = null;
			selectedCells = 0;

			//reset the scene view tool to what it was befor we started selecting cells
			Tools.current = lastTool == Tool.None ? Tools.current : lastTool;
			lastTool = Tool.None;
		}


		// draw grid lines in scene view
		Handles.color = Color.black;
		Handles.color = Handles.color*new Color(1,1,1,0.75f);

		// draw vertical grid lines left to right
		for(int i = 0; i <= view.Dimensions.X; i++)
		{
			Vector3 point = transform.position + new Vector3(i*view.CellSpacingFactor.x,0,0) + Vector3.up*0.01f;
			Handles.DrawDottedLine(point, point + Vector3.forward*view.CellSpacingFactor.y*view.Dimensions.Y,4);
		}
		
		// draw horizontal grid lines back to front
		for(int i = 0; i <= view.Dimensions.Y; i++)
		{
			Vector3 point = transform.position + new Vector3(0,0,i*view.CellSpacingFactor.y) + Vector3.up*0.01f;
			Handles.DrawDottedLine(point, point + Vector3.right*view.CellSpacingFactor.x*view.Dimensions.X,4);
		}
	}
}
#endif