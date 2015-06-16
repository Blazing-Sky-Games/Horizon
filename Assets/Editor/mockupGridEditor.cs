using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mockupcell))]
[CanEditMultipleObjects]
public class mockupGridEditor : Editor 
{
	void OnSceneGUI()
	{
		if(Selection.activeObject != null && target.name == Selection.activeObject.name)
		{
			//Debug.Log(Event.current.shift);
			select3d.shift = Event.current.shift;
		}

		Mockupcell cell = (Mockupcell)target;
		cell.__UpdatePresentation();
		cell.passable
	}
}
