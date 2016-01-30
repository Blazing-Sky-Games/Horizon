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

public class YamlComponent
{
	public string GetValue(string key)
	{
		return ((YamlScalarNode)m_compNode.Children[new YamlScalarNode(key)]).Value;
	}

	public YamlComponent(MonoScript type, YamlMappingNode compNode)
	{
		m_type = type;
		m_compNode = compNode;
	}

	public MonoScript ComponentScript { get{ return m_type;} }

	private MonoScript m_type;
	private YamlMappingNode m_compNode;
}



