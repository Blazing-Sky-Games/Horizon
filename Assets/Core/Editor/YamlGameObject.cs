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

public class YamlGameObject
{
	public void SetValues(string name, IEnumerable<YamlComponent> components)
	{
		m_name = name;
		m_components = components;
	}

	public string Name { get{ return m_name;} }

	public IEnumerable<YamlComponent> GetComponents()
	{
		return m_components;
	}

	string m_name;
	IEnumerable<YamlComponent> m_components;
}


