using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Horizon.Combat.Models;

namespace Horizon.Combat.Editor
{
	public class AddUnitToCellsWindow : EditorWindow
	{
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
						//todo: replace with somthing like addunit() function
						unitInstance.transform.parent = cell.transform;
						unitInstance.grid = cell.grid;
						unitInstance.GridPosition = cell.GridPosition;
					}
				}
				Close();
			}
		}

		[MenuItem("Horizon/Add Unit To Cells")]
		static void Init()
		{
			EditorWindow.GetWindow<AddUnitToCellsWindow>().Close();
			EditorWindow.GetWindow<AddUnitToCellsWindow>();
		}
	}
}

