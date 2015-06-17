using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mockupcell))]
[CanEditMultipleObjects]
public class mockupCellEditor : Editor 
{
	void OnSceneGUI()
	{
		if(Selection.activeObject != null && target.name == Selection.activeObject.name)
		{
			select3d.shift = Event.current.shift;
		}
	}
}
