using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Gamelogic.Editor;
using Gamelogic.Grids;
using Gamelogic.Grids.Editor.Internal;

using System.Collections;

[CustomEditor(typeof(HorizonGridView))]
public class HorizonGridEditor : SimpleGridEditor<HorizonGridView, RectPoint> 
{
	new void OnEnable()
	{
		dimensionsProp = FindProperty("dimensions");
		lineMatProp = FindProperty("lineMaterial");
		
		var assetGuids = AssetDatabase.FindAssets("HorizonCell t:prefab");
		if(assetGuids.Length < 1)
		{
			Debug.LogError("HorizonCell Prefab Missing!");
			Target.CellPrefab = null;
		}
		else
		{
			GameObject HorizonCellPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuids[0]),typeof(GameObject));
			Target.CellPrefab = HorizonCellPrefab.GetComponent<HorizonCellView>();
		}
	}

	// serilized properties
	private GLSerializedProperty dimensionsProp;
	private GLSerializedProperty lineMatProp;
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		AddField(lineMatProp);
		AddField(dimensionsProp);
		
		if (GUI.changed)
		{
			CheckDimensions(dimensionsProp);
			serializedObject.ApplyModifiedProperties();
			
			Target.__UpdatePresentation(true);
		}
	}

	private static void CheckPositive(GLSerializedProperty property)
	{
		if (property.intValue < 1)
		{
			property.intValue = 1;
		}
	}
	
	private static void CheckDimensions(GLSerializedProperty property)
	{
		CheckPositive(property.FindPropertyRelative("x"));
		CheckPositive(property.FindPropertyRelative("y"));
	}
}
