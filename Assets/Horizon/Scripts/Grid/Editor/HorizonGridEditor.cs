using System.Collections;
using Gamelogic.Editor;
using Gamelogic.Grids;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Gamelogic.Grids.Editor.Internal;

// this is the custom inspector for grid stuff
// this just gives a way to edit the size of a grid
// do we even really need this ...
[CustomEditor(typeof(HorizonGridGameView))]
public class HorizonGridEditor : SimpleGridEditor<HorizonGridGameView, RectPoint> 
{
	new void OnEnable()
	{
		dimensionsProp = FindProperty("dimensions");
		lineMatProp = FindProperty("lineMaterial");
		
		// set the cell prefab
		// the cell prefab is the cell that gets created in the editor
		var assetGuids = AssetDatabase.FindAssets("HorizonCell t:prefab"); // the horizon cell prefab
		// hmm ... maybe i should be doing this editor black magic, and just have the user assign the prefabe in the editor them self
		if(assetGuids.Length < 1)
		{
			Debug.LogError("HorizonCell Prefab Missing!");
			Target.CellPrefab = null;
		}
		else
		{
			GameObject HorizonCellPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuids[0]),typeof(GameObject));
			Target.CellPrefab = HorizonCellPrefab.GetComponent<HorizonCellGameView>();
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
			CheckDimensions(dimensionsProp); // make sure the grid dimentions are in bounds
			serializedObject.ApplyModifiedProperties();
			
			Target.__UpdatePresentation(true); // update the visual properties of the grid
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
