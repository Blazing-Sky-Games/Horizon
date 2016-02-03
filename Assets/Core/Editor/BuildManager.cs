using System;
using UnityEditor;
using System.Linq;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class BuildManager
{
	[MenuItem("Build/Build Scene List")]
	public static void BuildSceneList()
	{
		var ScenesInBuild = EditorBuildSettings.scenes;
		string[] levels = ScenesInBuild.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();

		YamlHelper yamlHelper = new YamlHelper();
		string contextSceneRecord = "";
		foreach(string scenePath in levels)
		{
			YamlScene scene = yamlHelper.LoadYamlScene(scenePath);

			YamlGameObject mainContextObject = scene.GetGameObjects().Where(go => go.GetComponents().Any(co => co.ComponentScript.GetClass() == typeof(MainContextHolder))).FirstOrDefault();
			if(mainContextObject != null)
			{
				YamlComponent comp = mainContextObject.GetComponents().Where(co => co.ComponentScript.GetClass() == typeof(MainContextHolder)).First();
				contextSceneRecord += comp.GetValue("contextType").Split(',').First() + " " + scenePath.Remove(scenePath.Length - ".unity".Count()).Remove(0,"Assets/".Count()) + " " + mainContextObject.Name + "\n";
			}
		}

		File.WriteAllText(Application.dataPath + "/Core/Resources/SceneList.txt", contextSceneRecord);
		AssetDatabase.Refresh();
	}
}