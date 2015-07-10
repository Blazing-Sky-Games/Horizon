﻿using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Horizon.Models;
using UnityEditor;
using UnityEngine;

namespace Horizon.Editor
{
	public static class StringExtensions
	{
		public static string SplitCamelCase(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			
			string camelCase = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();
			
			if (str.Length > 1)
			{
				string rest = camelCase.Substring(1);
				
				return firstLetter + rest;
			}
			else
			{
				return firstLetter;
			}
		}
	}

	//TODO: support multiobject editing. use showmixedvalue
	[CustomEditor(typeof( HorizonBaseModel ), true, isFallback = true)]
	public class HorizonBaseEditor : UnityEditor.Editor
	{
		private PropertyInfo[] properties;
		private bool showDefault = false;
		private bool showProps = false;

		private void propertyUiDisplayHelper<T>(PropertyInfo property, Func<string,T,GUILayoutOption[],T> uiFunction)
		{
			property.GetSetMethod().Invoke(
				target,
				new System.Object[]{
					uiFunction(
						property.Name.SplitCamelCase(),
						(T)property.GetValue(target,null),
						new GUILayoutOption[]{}
					)
				}
			);
		}

		// display the name of a property and ui to edit it
		protected void PropertyFieldFromInfo(PropertyInfo property)
		{
			if(property.CanRead && property.CanWrite)
			{
				//TODO add ability to extend what classes can be shown from outside this file

				// display value editor ui based on property type
				if(property.PropertyType == typeof(Bounds))
				{
					propertyUiDisplayHelper<Bounds>(property, EditorGUILayout.BoundsField);
				}
				else if(property.PropertyType == typeof(Color))
				{
					propertyUiDisplayHelper<Color>(property, EditorGUILayout.ColorField);
				}
				else if(property.PropertyType == typeof(AnimationCurve))
				{
					if(property.GetValue(target,null) != null)
						propertyUiDisplayHelper<AnimationCurve>(property, EditorGUILayout.CurveField);
				}
				else if(property.PropertyType == typeof(double))
				{
					propertyUiDisplayHelper<double>(property, EditorGUILayout.DoubleField);
				}
				else if(property.PropertyType.IsEnum)
				{
					if(property.PropertyType.GetCustomAttributes(typeof(FlagsAttribute),true).Length > 0)
					{
						propertyUiDisplayHelper<Enum>(property, EditorGUILayout.EnumMaskField);
					}
					else
					{
						propertyUiDisplayHelper<Enum>(property, EditorGUILayout.EnumPopup);
					}
				}
				else if(property.PropertyType == typeof(float))
				{
					propertyUiDisplayHelper<float>(property, EditorGUILayout.FloatField);
				}
				else if(property.PropertyType == typeof(int))
				{
					propertyUiDisplayHelper<int>(property, EditorGUILayout.IntField);
				}
				else if(property.PropertyType == typeof(LayerMask))
				{
					propertyUiDisplayHelper<LayerMask>(property, (label,layer,options) => (LayerMask)EditorGUILayout.LayerField(label,layer.value,options));
				}
				else if(property.PropertyType == typeof(long))
				{
					propertyUiDisplayHelper<long>(property, EditorGUILayout.LongField);
				}
				else if(property.PropertyType == typeof(Rect))
				{
					propertyUiDisplayHelper<Rect>(property, EditorGUILayout.RectField);
				}
				else if(property.PropertyType == typeof(string))
				{
					propertyUiDisplayHelper<string>(property, EditorGUILayout.TextField);
				}
				else if(property.PropertyType == typeof(bool))
				{
					propertyUiDisplayHelper<bool>(property, EditorGUILayout.Toggle);
				}
				else if(property.PropertyType == typeof(Vector2))
				{
					propertyUiDisplayHelper<Vector2>(property, EditorGUILayout.Vector2Field);
				}
				else if(property.PropertyType == typeof(Vector3))
				{
					propertyUiDisplayHelper<Vector3>(property, EditorGUILayout.Vector3Field);
				}
				else if(property.PropertyType == typeof(Vector4))
				{
					propertyUiDisplayHelper<Vector4>(property, EditorGUILayout.Vector4Field);
				}
				else if( property.PropertyType == typeof(UnityEngine.Object) || property.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					propertyUiDisplayHelper<UnityEngine.Object>(property, (label,obj,options) => EditorGUILayout.ObjectField(label,obj,property.PropertyType,true,options));
				}
				else if(property.PropertyType.GetInterfaces().Contains(typeof(IList)))
				{
					//TODO
				}
				else if(property.PropertyType.GetInterfaces().Contains(typeof(IDictionary)))
				{
					//TODO
				}
				else if(property.PropertyType.GetInterfaces().Contains(typeof(ICollection)))
				{
					//TODO
					//inotifycollectionchanged
					//etc
					//hmm ... we would have to make weaksubscription able to listen to collections
					//hmm ... is having a collection as a property common?
				}
				else if(property.PropertyType == typeof(Type))
				{
					//TODO
					//ooo, how meta
					//display a dropdown that lets them select a type from all types in all assemblys
					//hmm ... or maybe just scriptable objects + monobehaviors? ...
					//hmm...i wish there was a way to restrict the kind of type
				}
				else
				{
					EditorGUILayout.LabelField(property.Name.SplitCamelCase(),"cannot display");
				}
				//hmm ... how to handle delegates ...
			}
		}

		// get the set of properties to display
		protected virtual void OnEnable()
		{
			Type ModelType = target.GetType();

			properties = ModelType
				.GetProperties()
				.Where(x => x.DeclaringType != typeof(UnityEngine.Object) 
					       	&& x.DeclaringType != typeof(Component) 
					       	&& x.DeclaringType != typeof(Behaviour) 
					       	&& x.DeclaringType != typeof(MonoBehaviour))
				.ToArray();
		}

		// show all properties
		public override void OnInspectorGUI()
		{
			//default inspector
			//TODO: replace by showing public fields with aditional logic (ie, better collection support, other cool stuff from the property function)
			if (showDefault = EditorGUILayout.Foldout(showDefault,"Default Inspector (public / serilized private)"))
			{
				DrawDefaultInspector();
			}

			//properties
			if (showProps = EditorGUILayout.Foldout(showProps,"Properties (edits fire accsesor logic)"))
			{
				foreach(PropertyInfo property in properties)
				{
					PropertyFieldFromInfo(property);
				}
			}

			//funtions
			//button to press to call a function, and fields to supply arguments if possible

			//events
			//ui to hookup handlers to events, like OnClick() in a button
		}
	}
}


