using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
public class ResourcePathPropertyDrawer : PropertyDrawer
{
	private static Dictionary<UnityEngine.Object,UnityEngine.Object> resources = new Dictionary<UnityEngine.Object, UnityEngine.Object>();

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if(!resources.ContainsKey(property.serializedObject.targetObject))
		{
			if(property.stringValue != "MISSINGPATH" && property.stringValue != "")
			{
				resources[property.serializedObject.targetObject] = Resources.Load(property.stringValue);
			}
			else
			{
				resources[property.serializedObject.targetObject] = null;
			}
		}

		UnityEngine.Object old = resources[property.serializedObject.targetObject];

		var target = property.serializedObject.targetObject;
		Type resType = target.GetType().GetField("ResType").GetValue(target) as Type;
		resources[property.serializedObject.targetObject] = EditorGUILayout.ObjectField(property.name,resources[property.serializedObject.targetObject], resType, false);

		if(resources[property.serializedObject.targetObject] == null 
			|| !AssetDatabase.Contains(resources[property.serializedObject.targetObject])
			|| !AssetDatabase.GetAssetPath(resources[property.serializedObject.targetObject]).Contains("/Resources/"))
		{
			property.stringValue = "MISSINGPATH";
		}
		else if(old == null || !old.Equals(resources[property.serializedObject.targetObject]))
		{
			string path = AssetDatabase.GetAssetPath(resources[property.serializedObject.targetObject]);
			path = path.Substring(path.IndexOf("/Resources/")).Remove(0,"/Resources/".Count());
			path = path.Remove(path.Count() - ".asset".Count());

			property.stringValue = path;
		}

		property.serializedObject.ApplyModifiedProperties();
	}
}


