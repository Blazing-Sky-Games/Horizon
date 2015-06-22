using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

using Gamelogic.Editor;
using Gamelogic.Grids;
using Gamelogic.Grids.Editor.Internal;

[CustomEditor(typeof(HorizonUnitModel))]
public class HorizonUnitEditor : GLEditor<HorizonUnitModel> 
{
	void OnEnable()
	{
		Target.GridView = GameObject.Find("HorizonGrid").GetComponent<HorizonGridView>();

		Target.PositionPoint = new RectPoint(Target.X,Target.Y);

		if(CheckPosition(Target.PositionPoint) == false)
		{
			Func<bool> findValidCellFunc = () => 
			{ 
				for(int i = 0; i < Target.GridView.Dimensions.X; i++)
				{
					for(int j = 0; j < Target.GridView.Dimensions.Y; j++)
					{
						if ( CheckPosition(new RectPoint(i,j)) )
						{
							Target.PositionPoint = new RectPoint(i,j);
							Target.X = i;
							Target.Y = j;
							Target.transform.position = Target.GridView.model.CellViewGrid[Target.PositionPoint].transform.position;
							return true;
						}
					}
				}

				return false;
			};

			if(findValidCellFunc() == false) Debug.LogError("nowhere to put a unit!");
		}

		switch(Target.Direction)
		{
		case UnitDirection.forward:
			Target.transform.rotation = Quaternion.identity;
			break;
		case UnitDirection.backward:
			Target.transform.rotation = Quaternion.Euler(0,180,0);
			break;
		case UnitDirection.left:
			Target.transform.rotation = Quaternion.Euler(0,-90,0);
			break;
		case UnitDirection.right:
			Target.transform.rotation = Quaternion.Euler(0,90,0);
			break;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		DrawDefaultInspector();
		
		int oldX = Target.X;
		int oldY = Target.Y;
		
		serializedObject.ApplyModifiedProperties();

		if (GUI.changed)
		{
			switch(Target.Direction)
			{
			case UnitDirection.forward:
				Target.transform.rotation = Quaternion.identity;
				break;
			case UnitDirection.backward:
				Target.transform.rotation = Quaternion.AngleAxis(180,Vector3.up);
				break;
			case UnitDirection.left:
				Target.transform.rotation = Quaternion.AngleAxis(-90,Vector3.up);
				break;
			case UnitDirection.right:
				Target.transform.rotation = Quaternion.AngleAxis(90,Vector3.up);
				break;
			}

			if ( CheckPosition(new RectPoint(Target.X,Target.Y)) )
			{
				Target.transform.position = Target.GridView.model.CellViewGrid[new RectPoint(Target.X,Target.Y)].transform.position;
				Target.PositionPoint = new RectPoint(Target.X,Target.Y);
			}
			else
			{
				Target.X = oldX;
				Target.Y = oldY;
			}
		}
	}
	
	private bool CheckPosition(RectPoint point)
	{
		if (Target.GridView == null) return false;

		if(Target.GridView.Grid.IsOutside(point)) return false;

		return Target.GridView.model.CellViewGrid[point].model.state == CellState.Passable;
	}
}
