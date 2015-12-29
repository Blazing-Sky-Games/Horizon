using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectWindow : EditorWindow
{
	private static Type[] Types
	{ 
		get { return m_types; }
		set
		{
			m_types = value;
			m_names = m_types.Select(t => t.FullName).ToArray();
		}
	}
	
	public static void Init(Type[] scriptableObjects)
	{
		Types = scriptableObjects;
		
		var window = EditorWindow.GetWindow<ScriptableObjectWindow>(true, "Create a new ScriptableObject", true);
		window.ShowPopup();
	}
	
	public void OnGUI()
	{
		GUILayout.Label("ScriptableObject Class");
		m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_names);
		
		if(GUILayout.Button("Create"))
		{
			var asset = ScriptableObject.CreateInstance(m_types[m_selectedIndex]);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
				asset.GetInstanceID(),
				ScriptableObject.CreateInstance<ScriptableObjectWindowEndNameEditAction>(),
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