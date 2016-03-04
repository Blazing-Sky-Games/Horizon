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
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ResourceReference<>),true)]
public class ResourceReferencePropertyDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		EditorGUI.indentLevel = 0;

		SerializedProperty pathProperty = property.FindPropertyRelative("ResourcePath");
		string path = pathProperty.stringValue;

		UnityEngine.Object resource = Resources.Load(path);

		IResoureReference propValue = (IResoureReference)property.serializedObject.targetObject.GetType().GetField(property.name).GetValue(property.serializedObject.targetObject);

		resource = EditorGUI.ObjectField(contentPosition, resource,propValue.ResType,false);

		if(resource != null && AssetDatabase.Contains(resource))
		{
			string truncatedPath = AssetDatabase.GetAssetPath(resource);
			truncatedPath = truncatedPath.Remove(0, truncatedPath.LastIndexOf("Resources/") + "Resources/".Count());
			truncatedPath = truncatedPath.Remove(truncatedPath.Count() - ".asset".Count());
			pathProperty.stringValue = truncatedPath;
		}

		EditorGUI.EndProperty();
	}
}


