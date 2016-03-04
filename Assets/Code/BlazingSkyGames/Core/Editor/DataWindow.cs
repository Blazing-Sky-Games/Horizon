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
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DataWindow : EditorWindow
{
	private static string GetTypeNameInCatagory(Type t)
	{
		string name = t.Name;

		//TODO catagorizing data for easyer creation
		//DataCatagory catagory = t.GetCustomAttributes (typeof(DataCatagory), false).FirstOrDefault () as DataCatagory;

		//if(catagory != null)
			//name = catagory.Catagory + "/" + name;

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