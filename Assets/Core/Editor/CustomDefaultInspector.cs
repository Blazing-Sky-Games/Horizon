using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(Data), true)]
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
		DoDrawDefaultInspector(serializedObject, true);
	}

	bool DoDrawDefaultInspector (SerializedObject obj, bool showName)
	{
		EditorGUI.BeginChangeCheck();
		obj.Update();
		SerializedProperty iterator = obj.GetIterator();
		bool enterChildren = true;
		while(iterator.NextVisible(enterChildren))
		{
			bool selectType;
			if(ShouldInlineProp(iterator, out selectType))
			{
				drawInline(iterator, showName, selectType);
			}
			else
			{
				if(HasCustomPropertyDrawer(iterator))
				{
					DrawWithCustomPropertyDrawer(iterator);
				}
				else
				{
					EditorGUILayout.PropertyField(iterator, true, new UnityEngine.GUILayoutOption[0]);
				}
			}
			enterChildren = false;
		}
		obj.ApplyModifiedProperties();
		return EditorGUI.EndChangeCheck();
	}

	bool HasCustomPropertyDrawer(SerializedProperty prop)
	{
		return GetCustomPropertyDrawerType(prop) != null;
	}

	Type GetCustomPropertyDrawerType(SerializedProperty prop)
	{
		FieldInfo field = prop.serializedObject.targetObject.GetType().GetField(prop.name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		if(field == null)
			return null;

		Type fieldType = field.FieldType;

		if(customPropertyDrawers.ContainsKey(fieldType))
		{
			return customPropertyDrawers[fieldType];
		}
		else if(field.GetCustomAttributes(typeof(PropertyAttribute), false).Count() != 0)
		{
			Type attribType = field.GetCustomAttributes(typeof(PropertyAttribute), false).First().GetType();

			if(customPropertyDrawers.ContainsKey(attribType))
			{
				return customPropertyDrawers[attribType];
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

	void DrawWithCustomPropertyDrawer(SerializedProperty prop)
	{
		Type drawerType = GetCustomPropertyDrawerType(prop);

		PropertyDrawer drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);

		FieldInfo field = prop.serializedObject.targetObject.GetType().GetField(prop.name);

		typeof(PropertyDrawer)
			.GetField("m_Attribute", BindingFlags.Instance|BindingFlags.NonPublic)
			.SetValue(drawer,field
				.GetCustomAttributes(typeof(PropertyAttribute),false)
				.FirstOrDefault());

		Rect rect = EditorGUILayout.GetControlRect(false, drawer.GetPropertyHeight(prop, new GUIContent()));

		drawer.OnGUI(rect,prop,null);
	}

	void drawInline (SerializedProperty prop, bool showName , bool selectType, Type baset = null)
	{
		if(prop.isArray)
		{
			drawList(prop, showName);
		}
		else
		{
			Type targetType = target.GetType();

			FieldInfo field = targetType.GetField(prop.name);

			Type baseType = baset ?? field.FieldType;

			if(selectType)
			{
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
			}
			else
			{
				if(prop.objectReferenceValue == null)
				{
					UnityEngine.Object newAsset = ScriptableObject.CreateInstance(baseType);
					string aPath = AssetDatabase.GetAssetPath(target);
					aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
					AssetDatabase.AddObjectToAsset(newAsset, aPath);

					prop.objectReferenceValue = newAsset;
				}

				prop.serializedObject.ApplyModifiedProperties();
			}

			if(prop.objectReferenceValue != null)
			{
				if(showName)
				{
					GUILayout.Label(prop.name);
				}
				EditorGUI.indentLevel++;
				DoDrawDefaultInspector(new SerializedObject(prop.Copy().objectReferenceValue),showName);
				EditorGUI.indentLevel--;
			}
		}
	}

	void drawList (SerializedProperty listProp, bool showName)
	{
		bool selectType;
		if(!ShouldInlineProp(listProp,out selectType))
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

			drawInline(elemi, false, selectType , field.FieldType.GetGenericArguments()[0]);
			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();
	}

	bool ShouldInlineProp (SerializedProperty prop, out bool selectType)
	{
		Type targetType = target.GetType();

		FieldInfo field = targetType.GetField(prop.name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		if(field == null)
		{
			selectType = false;
			return false;
		}

		bool shouldInlineList = field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && shouldInlinetype(field.FieldType.GetGenericArguments()[0],out selectType);
		return shouldInlineList || shouldInlinetype(field.FieldType, out selectType);
	}

	bool shouldInlinetype (Type t, out bool selectType)
	{
		bool iSdata = typeof(Data).IsAssignableFrom(t);
		bool isInline = t.GetCustomAttributes(typeof(InlineData), true).Count() > 0;

		selectType = iSdata && isInline && (t.GetCustomAttributes(typeof(InlineData), true).First() as InlineData).SelectType;
		return iSdata && isInline;
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

	static CustomDefaultInspector()
	{
		ServiceLocator.RegisterService<IReflectionService,ReflectionService>();
		var refService = ServiceLocator.GetService<IReflectionService>();

		var drawers = refService.GetDerivedTypes(typeof(PropertyDrawer)).derivedTypes.Select(x => x.Value).ToList();
		drawers = drawers.Where(x => x.GetCustomAttributes(typeof(CustomPropertyDrawer), false).Count() != 0).ToList();
		var drawAttribs = drawers.Select(x => x.GetCustomAttributes(typeof(CustomPropertyDrawer), false).First() as CustomPropertyDrawer).ToList();

		FieldInfo field = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.Instance|BindingFlags.NonPublic);

		var drawAtrribtypes = drawAttribs.Select(x => field.GetValue(x) as Type).ToList();

		for(int i = 0; i < drawAtrribtypes.Count; i++)
		{
			customPropertyDrawers[drawAtrribtypes[i]] = drawers[i];
		}
	}

	private static Dictionary<Type, Type> customPropertyDrawers = new Dictionary<Type, Type>();

	private IReflectionService reflectionService;
}


