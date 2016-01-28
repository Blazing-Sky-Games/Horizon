using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(UnityEngine.Object), true)]
public class CustomDefaultInspector : Editor
{
	public void OnEnable ()
	{
		//TODO fix this
		ServiceLocator.RegisterService<IReflectionService,ReflectionService>();
		reflectionService = ServiceLocator.GetService<IReflectionService>();
	}

	public override void OnInspectorGUI ()
	{
		DoDrawDefaultInspector(serializedObject);
	}

	bool DoDrawDefaultInspector (SerializedObject obj)
	{
		EditorGUI.BeginChangeCheck();
		obj.Update();
		SerializedProperty iterator = obj.GetIterator();
		bool enterChildren = true;
		while(iterator.NextVisible(enterChildren))
		{
			if(ShouldInlineProp(iterator))
			{
				drawInline(iterator);
			}
			else
			{
				EditorGUILayout.PropertyField(iterator, true, new UnityEngine.GUILayoutOption[0]);
			}
			enterChildren = false;
		}
		obj.ApplyModifiedProperties();
		return EditorGUI.EndChangeCheck();
	}

	void drawInline (SerializedProperty prop, Type baset = null)
	{
		if(prop.isArray)
		{
			drawList(prop);
		}
		else
		{

			Type targetType = target.GetType();

			FieldInfo field = targetType.GetField(prop.name);

			Type baseType = baset ?? field.FieldType;

			Type currentType;
			if(prop.objectReferenceValue == null)
			{
				currentType = reflectionService.GetDerivedTypes(baseType).derivedTypes[0].Value;
			}
			else
			{
				currentType = prop.objectReferenceValue.GetType();
			}

			Type SelectedType = currentType;

			SelectedType = TypeSelectPopup(baseType, SelectedType);

			if(prop.objectReferenceValue == null || prop.objectReferenceValue.GetType() != SelectedType)
			{
				if(prop.objectReferenceValue != null)
					DestroyImmediate(prop.objectReferenceValue, true);
				
				UnityEngine.Object newAsset = ScriptableObject.CreateInstance(SelectedType);
				string aPath = AssetDatabase.GetAssetPath(target);
				aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
				AssetDatabase.AddObjectToAsset(newAsset, aPath);

				prop.objectReferenceValue = newAsset;
			}

			prop.serializedObject.ApplyModifiedProperties();

			if(prop.objectReferenceValue != null)
			{
				EditorGUI.indentLevel++;
				DoDrawDefaultInspector(new SerializedObject(prop.Copy().objectReferenceValue));
				EditorGUI.indentLevel--;
			}
		}
	}

	void drawList (SerializedProperty listProp)
	{
		if(!ShouldInlineProp(listProp))
		{
			EditorGUILayout.PropertyField(listProp, true, new GUILayoutOption[0]);
			return;
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField(listProp.name);

		EditorGUI.indentLevel++;
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
			EditorGUILayout.Space();
			EditorGUI.indentLevel++;
			SerializedProperty elemi = listProp.GetArrayElementAtIndex(i);

			Type targetType = target.GetType();

			FieldInfo field = targetType.GetField(listProp.name);

			drawInline(elemi, field.FieldType.GetGenericArguments()[0]);
			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();
	}

	bool ShouldInlineProp (SerializedProperty prop)
	{

		Type targetType = target.GetType();

		FieldInfo field = targetType.GetField(prop.name);

		if(field == null)
			return false;

		bool shouldInlineList = field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && shouldInlinetype(field.FieldType.GetGenericArguments()[0]);
		return shouldInlinetype(field.FieldType) || shouldInlineList;
	}

	bool shouldInlinetype (Type t)
	{
		return typeof(ScriptableObject).IsAssignableFrom(t) && (t.GetCustomAttributes(typeof(InlineData), true).Count() > 0 || t.GetCustomAttributes(typeof(InlineData), true).Count() > 0);
	}

	Type TypeSelectPopup (Type baseType, Type selected)
	{
		if(!baseType.IsAssignableFrom(selected) || selected.IsAbstract)
			throw new Exception();

		var derivedTypes = reflectionService.GetDerivedTypes(baseType);

		int selectedIndex = derivedTypes.typeIndexLookup[selected];

		selectedIndex = EditorGUILayout.Popup("Type", selectedIndex, derivedTypes.derivedTypes.Select(x => x.Key).ToArray());

		return derivedTypes.derivedTypes[selectedIndex].Value;
	}

	private IReflectionService reflectionService;
}


