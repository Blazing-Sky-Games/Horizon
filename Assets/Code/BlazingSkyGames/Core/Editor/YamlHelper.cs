/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
