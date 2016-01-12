using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DataWindow : EditorWindow
{
	private static string GetTypeNameInCatagory(Type t)
	{
		string name = t.Name;

		DataCatagory catagory = t.GetCustomAttributes (typeof(DataCatagory), false).FirstOrDefault () as DataCatagory;

		if(catagory != null)
			name = catagory.Catagory + "/" + name;

		return name;
	}

	private static Type[] Types
	{ 
		get { return m_types; }
		set
		{
			m_types = value;
			m_names = m_types.Select(t => GetTypeNameInCatagory(t)).ToArray();
		}
	}
	
	public static void Init(Type[] dataTypes)
	{
		Types = dataTypes;
		
		var window = EditorWindow.GetWindow<DataWindow>(true, "Create new data", true);
		window.ShowPopup();
	}
	
	public void OnGUI()
	{
		GUILayout.Label("Data Class");
		m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_names);
		
		if(GUILayout.Button("Create"))
		{
			var asset = ScriptableObject.CreateInstance(m_types[m_selectedIndex]);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
				asset.GetInstanceID(),
				ScriptableObject.CreateInstance<DataWindowEndNameEditAction>(),
				string.Format("{0}.asset", m_names[m_selectedIndex]),
				AssetPreview.GetMiniThumbnail(asset), 
				null);
			
			Close();
		}
	}

	private int m_selectedIndex;
	private static string[] m_names;
	private static Type[] m_types;
}