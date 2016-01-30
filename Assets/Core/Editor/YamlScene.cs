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

public class YamlScene
{
	public void SetGameObjects(IEnumerable<YamlGameObject> gameObjects)
	{
		m_gameObjects = gameObjects;

	}

	public IEnumerable<YamlGameObject> GetGameObjects()
	{
		return m_gameObjects;
	}

	IEnumerable<YamlGameObject> m_gameObjects;
}

