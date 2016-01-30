using System;
using UnityEditor;
using System.Linq;
using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;

public class YamlHelper
{
	public YamlScene LoadYamlScene(string path)
	{
		//Setup the input
		var input = new StringReader(File.ReadAllText(Path.Combine(Application.dataPath , path.Remove(0,"Assets/".Count()))));

		// Load the stream
		var yaml = new YamlStream();
		yaml.Load(input);

		Dictionary<string,YamlMappingNode> documents = new Dictionary<string,YamlMappingNode>();

		foreach(var document in yaml.Documents)
		{
			documents[document.RootNode.Anchor] = ((YamlMappingNode)document.RootNode);
		}

		Dictionary<YamlMappingNode, YamlGameObject> gameObjects = new Dictionary<YamlMappingNode, YamlGameObject>();

		foreach(var document in documents.Values)
		{
			string objectType = ((YamlScalarNode)document.Children.First().Key).Value;

			if(objectType == "GameObject")
			{
				gameObjects[(YamlMappingNode)document.Children.First().Value] = new YamlGameObject();
			}
		}

		foreach(var entry in gameObjects)
		{
			string name = ((YamlScalarNode)entry.Key.Children[new YamlScalarNode("m_Name")]).Value;
				
			List<YamlComponent> components = new List<YamlComponent>();

			YamlSequenceNode ComponentNodes = (YamlSequenceNode)entry.Key.Children[new YamlScalarNode("m_Component")];

			foreach(var componentNode in ComponentNodes.Children)
			{
				string anchor = ((YamlMappingNode)componentNode).Children.Values
					.Select(valueNode => (YamlMappingNode)valueNode)
					.Select(mappingNode => mappingNode.Children.First().Value)
					.Select(valueNode => (YamlScalarNode)valueNode)
					.Select(scalarNode => scalarNode.Value)
					.First();

				YamlMappingNode componentMap = (YamlMappingNode)documents[anchor].Children.First().Value;
				if(componentMap.Children.ContainsKey(new YamlScalarNode("m_Script")))
				{
					string scriptGuid = ((YamlScalarNode)((YamlMappingNode)componentMap.Children[new YamlScalarNode("m_Script")]).Children[new YamlScalarNode("guid")]).Value;

					string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);

					MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(scriptPath, typeof(MonoScript));

					components.Add(new YamlComponent(script, componentMap));
				}
			}

			entry.Value.SetValues(name, components);
		}

		YamlScene scene = new YamlScene();
		scene.SetGameObjects(gameObjects.Values);

		return scene;
	}
}
