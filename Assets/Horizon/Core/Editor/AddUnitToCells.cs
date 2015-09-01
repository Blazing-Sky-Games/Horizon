using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Horizon.Combat.Models;
namespace Horizon.Core.Editor
{
	public class AddUnitToCells : EditorWindow
	{
		[MenuItem("Assets/Create/GO Prefab")]
		static void NewEmptyPrefab()
		{
			GameObject go = new GameObject();

			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "")
			{
				path = "Assets";
			}
			else if (Path.GetExtension(path) != "")
			{
				path = path.Replace(Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}

			PrefabUtility.CreatePrefab(AssetDatabase.GenerateUniqueAssetPath(path + "/New Prefab.prefab"),go);

			DestroyImmediate(go);
		}

		private bool init = false;

		void OnGUI()
		{
			if(!init)
			{
				int controlID = EditorGUIUtility.GetControlID (FocusType.Passive);
				EditorGUIUtility.ShowObjectPicker<GameObject>(null,false,"",controlID);
				init = true;
			}

			if(Event.current.commandName == "ObjectSelectorUpdated") Debug.Log("yo");

			if(Event.current.commandName == "ObjectSelectorClosed")
			{
				GameObject unit = (GameObject)EditorGUIUtility.GetObjectPickerObject();
				if(unit != null && unit.GetComponent<Unit>() != null)
				{
					foreach(Cell cell in Selection.gameObjects.Where(go => go.GetComponent<Cell>() != null).Select(go => go.GetComponent<Cell>()))
					{
						Unit unitInstance = (PrefabUtility.InstantiatePrefab(unit) as GameObject).GetComponent<Unit>();
						unitInstance.gameObject.name = unit.name;
						unitInstance.transform.parent = cell.grid.transform;
						unitInstance.grid = cell.grid;
						unitInstance.GridPosition = cell.GridPosition;
					}
				}
				Close();
			}
		}

		[MenuItem("Horizon/Combat/Add Unit To Cells")]
		static void Init()
		{
			EditorWindow.GetWindow<AddUnitToCells>().Close();
			EditorWindow.GetWindow<AddUnitToCells>();
		}
	}
}

