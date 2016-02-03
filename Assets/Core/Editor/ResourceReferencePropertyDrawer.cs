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


