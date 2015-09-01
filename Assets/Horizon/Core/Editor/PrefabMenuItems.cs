
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
	public static class PrefabMenuItems
	{
		[MenuItem("Assets/Create/empty GO Prefab")]
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
			
			GameObject.DestroyImmediate(go);
		}
	}
}

