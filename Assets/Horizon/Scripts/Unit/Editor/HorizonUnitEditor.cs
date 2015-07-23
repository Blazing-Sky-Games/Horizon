using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

using Gamelogic.Editor;
using Gamelogic.Grids;
using Gamelogic.Grids.Editor.Internal;

// editor for the unit model
[CustomEditor(typeof(HorizonUnitModel))]
public class HorizonUnitModelEditor : GLEditor<HorizonUnitModel> 
{
	void OnEnable()
	{
		Target.GridView = GameObject.Find("HorizonGrid").GetComponent<HorizonGridGameView>();

		// set position
		Target.PositionPoint = new RectPoint(Target.X,Target.Y);

		// make sure the unit is on a valid cell
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

		//set direction
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
			//set unit direction
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

			// set position
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

//editor for the unit view
//all this is for is fixing unit models so the highlight shader works
[CustomEditor(typeof(HorizonUnitGameView))]
[CanEditMultipleObjects]
public class HorizonUnitViewEditor : GLEditor<HorizonUnitGameView> 
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawDefaultInspector();

		if(GUILayout.Button("fix Model Outline"))
		{
			fixMeshForOutline(Target.gameObject.GetComponentInChildren<MeshFilter>());
		}
		
		serializedObject.ApplyModifiedProperties();
	}

	//HACK this is how i make the outline shader work for models with hard normals
	// it does a bunch of munging on the model to compute smoooth normals, and secretly passes them down to the shader
	// using the tagent vertex property
	// if you want to know more about this talk to me, matthew draper
	void fixMeshForOutline (MeshFilter filter) 
	{
		Mesh mesh = filter.sharedMesh;
		mesh = Instantiate(mesh);
		
		Dictionary<Vector3, List<int>> indexesForVertex = new Dictionary<Vector3, List<int>>();
		
		for(int i = 0; i < mesh.vertices.Length; i++)
		{
			if(indexesForVertex.ContainsKey(mesh.vertices[i]) == false) indexesForVertex[mesh.vertices[i]] = new List<int>();
			
			indexesForVertex[mesh.vertices[i]].Add(i);
		}
		
		Vector4[] smoothNormals = new Vector4[mesh.normals.Length];
		foreach(KeyValuePair<Vector3, List<int>> vertex in indexesForVertex)
		{
			Vector3 normalSum = Vector3.zero;
			foreach(int index in vertex.Value)
			{
				normalSum += mesh.normals[index];
			}
			Vector3 smoothNormal = (normalSum/vertex.Value.Count).normalized;
			
			foreach(int index in vertex.Value)
			{
				smoothNormals[index] = new Vector4(smoothNormal.x,smoothNormal.y,smoothNormal.z,0);
			}
		}
		
		mesh.tangents = smoothNormals;
		
		filter.sharedMesh = mesh;
	}
}
