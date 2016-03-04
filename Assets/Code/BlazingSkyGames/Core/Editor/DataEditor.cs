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
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(Data), true)]
public class DataEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		DoDrawDefaultInspector(serializedObject);
	}

	//copied from untiy decompiled
	bool DoDrawDefaultInspector (SerializedObject obj)
	{
		EditorGUI.BeginChangeCheck();
		obj.Update();
		SerializedProperty iterator = obj.GetIterator();
		bool enterChildren = true;
		while(iterator.NextVisible(enterChildren))
		{
			DrawProperty(iterator, true);
			enterChildren = false;
		}
		obj.ApplyModifiedProperties();
		return EditorGUI.EndChangeCheck();
	}

	void DrawProperty (SerializedProperty prop, bool includeChildren, params GUILayoutOption[] options)
	{
		GUIContent label = new GUIContent(prop.displayName);

		//if the propoerty is polymorphic or a polymorphic list...
		if(IsPolymorphic(prop))
		{
			// ... draw it inline
			DrawPolymorphic(prop);
		}
		// else, try to draw it with a property drawer
		else if(HasCustomPropertyDrawer(prop))
		{
			DrawWithCustomPropertyDrawer(prop);
		}
		//else, just use the default drawer
		else
		{
			EditorGUILayout.PropertyField(prop, label, includeChildren, options);
		}
	}

	//is this property a polymorphic serilizable type, or a list of such?
	bool IsPolymorphic (SerializedProperty prop)
	{
		Type targetType = target.GetType();

		FieldInfo field = targetType.GetField(prop.name);

		if(field == null)
		{
			return false;
		}

		bool fieldIsList = field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>);

		if(fieldIsList)
		{
			return IsPolymorphic(field.FieldType.GetGenericArguments()[0]);
		}
		else
		{
			return IsPolymorphic(field.FieldType);
		}
	}

	bool IsPolymorphic (Type t)
	{
		return t.IsSubclassOf(typeof(PolymorphicSerializable));
	}

	void DrawPolymorphic (SerializedProperty prop, Type BaseType = null)
	{
		Rect rect = EditorGUILayout.GetControlRect(true);
		EditorGUI.BeginProperty(rect,new GUIContent(prop.displayName),prop);
		if(prop.isArray)
		{
			DrawPolymorphicList(prop);
		}
		else
		{
			EditorGUILayout.LabelField(prop.displayName);

			FieldInfo field = prop.serializedObject.targetObject.GetType().GetField(prop.name);

			Type baseT = BaseType ?? field.FieldType;

			// the type of the feilds value, or the default type if it is null
			Type SelectedType = prop.objectReferenceValue == null ?
				HorizonReflectionUtility.GetDerivedTypeRecord(baseT).Types.First() :
				prop.objectReferenceValue.GetType();

			// select what type of object you want this feild to be
			SelectedType = TypeSelectPopup(baseT, SelectedType);

			// if the object is null or the feild type has been changed
			if(prop.objectReferenceValue == null || prop.objectReferenceValue.GetType() != SelectedType)
			{
				//delete the old asset
				if(prop.objectReferenceValue != null)
					DestroyImmediate(prop.objectReferenceValue, true);

				//creat a new asset of the selected type
				UnityEngine.Object newAsset = ScriptableObject.CreateInstance(SelectedType);
				string aPath = AssetDatabase.GetAssetPath(target);
				AssetDatabase.AddObjectToAsset(newAsset, aPath);

				//assign the new asset to the field
				prop.objectReferenceValue = newAsset;
				prop.serializedObject.ApplyModifiedProperties();
				AssetDatabase.Refresh();
			}

			//draw the value of the object
			DoDrawDefaultInspector(new SerializedObject(prop.Copy().objectReferenceValue));
		}
		EditorGUI.EndProperty();
	}

	void DrawPolymorphicList (SerializedProperty listProp)
	{
		EditorGUILayout.LabelField(listProp.displayName);

		int listSize = listProp.arraySize;
		listSize = EditorGUILayout.IntField("size", listSize);

		if(listSize != listProp.arraySize)
		{
			while(listSize > listProp.arraySize)
			{
				listProp.InsertArrayElementAtIndex(listProp.arraySize);
				var sp = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
				sp.objectReferenceValue = null;
			}
			while(listSize < listProp.arraySize)
			{
				DestroyImmediate(listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue, true);
				listProp.DeleteArrayElementAtIndex(listProp.arraySize - 1);
			}
		}

		for(int i = 0; i < listProp.arraySize; i++)
		{
			FieldInfo field = listProp.serializedObject.targetObject.GetType().GetField(listProp.name);
			Type itemType = field.FieldType.GetGenericArguments()[0];

			SerializedProperty elemi = listProp.GetArrayElementAtIndex(i);
			DrawPolymorphic(elemi, itemType);
		}
	}

	Type TypeSelectPopup (Type baseType, Type selectedType)
	{
		DerivedTypeRecord derivedTypeRecord = HorizonReflectionUtility.GetDerivedTypeRecord(baseType);
		int selectedIndex = derivedTypeRecord.IndexOfType(selectedType);

		selectedIndex = EditorGUILayout.Popup("Type", selectedIndex, derivedTypeRecord.TypeNames.ToArray());

		return derivedTypeRecord.Types.ToArray()[selectedIndex];
	}

	bool HasCustomPropertyDrawer (SerializedProperty prop)
	{
		return GetCustomPropertyDrawerType(prop) != null;
	}

	Type GetCustomPropertyDrawerType (SerializedProperty prop)
	{
		FieldInfo field = prop.serializedObject.targetObject.GetType().GetField(prop.name);

		if(field == null)
			return null;

		if(customPropertyDrawerForType.ContainsKey(field.FieldType))
		{
			return customPropertyDrawerForType[field.FieldType];
		}
		else if(field.GetCustomAttributes(typeof(PropertyAttribute), false).Count() != 0)
		{
			Type attribType = field.GetCustomAttributes(typeof(PropertyAttribute), false).First().GetType();

			if(customPropertyDrawerForType.ContainsKey(attribType))
			{
				return customPropertyDrawerForType[attribType];
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
	}

	void DrawWithCustomPropertyDrawer (SerializedProperty prop)
	{
		GUIContent label = new GUIContent(prop.displayName);

		Type drawerType = GetCustomPropertyDrawerType(prop);

		PropertyDrawer drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);

		FieldInfo field = prop.serializedObject.targetObject.GetType().GetField(prop.name);

		typeof(PropertyDrawer)
			.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic)
			.SetValue(drawer, field
				.GetCustomAttributes(typeof(PropertyAttribute), false)
				.FirstOrDefault());

		Rect rect = EditorGUILayout.GetControlRect(false, drawer.GetPropertyHeight(prop, label));

		drawer.OnGUI(rect, prop, label);
	}

	static DataEditor()
	{
		var drawers = HorizonReflectionUtility.GetDerivedTypeRecord(typeof(PropertyDrawer)).Types.ToList();
		drawers = drawers.Where(x => x.GetCustomAttributes(typeof(CustomPropertyDrawer), false).Count() != 0).ToList();
		var drawAttribs = drawers.Select(x => x.GetCustomAttributes(typeof(CustomPropertyDrawer), false).First() as CustomPropertyDrawer).ToList();

		FieldInfo field = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);

		var drawAtrribtypes = drawAttribs.Select(x => field.GetValue(x) as Type).ToList();

		for(int i = 0; i < drawAtrribtypes.Count; i++)
		{
			//customPropertyDrawerForType[drawAtrribtypes[i]] = drawers[i];

			var derivedTypes = HorizonReflectionUtility.GetDerivedTypeRecord(drawAtrribtypes[i]);

			for(int j = 0; j < derivedTypes.Types.Count(); j++)
			{
				customPropertyDrawerForType[derivedTypes.Types.ToArray()[j]] = drawers[i];
			}
		}
	}

	private static Dictionary<Type, Type> customPropertyDrawerForType = new Dictionary<Type, Type>();
}


