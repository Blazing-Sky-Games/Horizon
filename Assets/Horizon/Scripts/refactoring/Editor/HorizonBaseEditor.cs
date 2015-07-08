using System;
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

	[CustomEditor( typeof( HorizonBaseModel ), true )]
	// display all properties in the model
	public class HorizonBaseEditor : UnityEditor.Editor
	{
		private PropertyInfo[] properties;

		// display the name of a property and ui to edit it
		protected void PropertyFieldFromInfo(PropertyInfo property)
		{
			if(property.CanRead && property.CanWrite)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(property.Name.SplitCamelCase());
				
				// display value editor ui based on property type
				if(property.PropertyType == typeof(int))
				{
					int value = EditorGUILayout.IntField((int)property.GetValue(target,null));
					property.GetSetMethod().Invoke(target,new System.Object[]{value});
				}
				else if(property.PropertyType.IsEnum)
				{
					Enum value = EditorGUILayout.EnumPopup((Enum)property.GetValue(target,null));
					property.GetSetMethod().Invoke(target,new System.Object[]{value});
				}
				else
				{
					GUILayout.Label(property.PropertyType.ToString());
				}

				EditorGUILayout.EndHorizontal();
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
			foreach(PropertyInfo property in properties)
			{
				PropertyFieldFromInfo(property);
			}
		}
	}
}


