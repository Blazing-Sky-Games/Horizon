using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using System.Collections.Generic;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core.Editor
{
	[CustomEditor(typeof( ModelBase ), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class ModelInspector : UnityEditor.Editor
	{
		//draw a grey dividing line in the inspector
		private static void Splitter()
		{
			float thickness = 1;

			GUIStyle SplitterStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.whiteTexture},
				stretchWidth = true,
				margin = new RectOffset(0, 0, 7, 7)
			};

			Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

			Rect position = GUILayoutUtility.GetRect(GUIContent.none, SplitterStyle, GUILayout.Height(thickness));
			
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = splitterColor;
				SplitterStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		private void DisplayObjectThroughReflection(UnityEngine.Object[] objs)
		{
			foreach(FieldInfo field in objs[0].GetType().GetFields().Where(x => x.GetCustomAttributes(typeof(HideInInspector),true).Count() == 0)) // dont show hidden fields
			{
				ModelInspectorUtility.DisplayMemberValue(field,objs);
			}

			var props =
				objs[0]
				.GetType()
				.GetProperties()
				.Where(
					x => x.DeclaringType != typeof(UnityEngine.Object) 
					&& x.DeclaringType != typeof(Component) 
					&& x.DeclaringType != typeof(Behaviour) 
					&& x.DeclaringType != typeof(MonoBehaviour)
				) // dont show inherited props
				.Where(x => x.GetCustomAttributes(typeof(HideInInspector),true).Count() == 0); // dont show hidden props

				foreach(PropertyInfo property in props)
				{
					if(property.CanRead && property.CanWrite)
					{
						ModelInspectorUtility.DisplayMemberValue(property,objs);
					}
				}
		}

		//hmm ... this is kinda hacky right now. this is to call the OnSceneGUI for all views of the target
		private void OnSceneGUI()
		{
			if(m_views == null) m_views = (List<ViewBaseNonGeneric>)(typeof(ModelBase).GetField("m_views", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target));

			foreach(var view in m_views)
			{
				view.OnSceneGUI();
			}
		}

		// show all properties and fields
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			GUIStyle boldAndItalicStyle = new GUIStyle();
			boldAndItalicStyle.fontStyle = FontStyle.BoldAndItalic;

			GUILayout.Label("Model",boldAndItalicStyle);

			DisplayObjectThroughReflection(targets);

			Splitter();
		
			GUILayout.Label("Views",boldAndItalicStyle);

			// stores the views of all selected models, based on type. this is so we can pass all of one type of view down to the display function
			Dictionary<Type, List<ViewBaseNonGeneric>> viewLookup = new Dictionary<Type, List<ViewBaseNonGeneric>>();
			foreach(object obj in targets)
			{
				if(m_views == null) m_views = (List<ViewBaseNonGeneric>)(typeof(ModelBase).GetField("m_views", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj));

				foreach(ViewBaseNonGeneric view in m_views)
				{
					if(viewLookup.ContainsKey(view.GetType()) == false) viewLookup[view.GetType()] = new List<ViewBaseNonGeneric>(); 

					viewLookup[view.GetType()].Add(view);
				}
			}

			//foreach type of view on this type of model, display the view
			foreach(KeyValuePair<Type, List<ViewBaseNonGeneric>> entry in viewLookup)
			{
				EditorGUILayout.InspectorTitlebar(true,entry.Value[0]);
				
				DisplayObjectThroughReflection(entry.Value.ToArray());
			}

			if(EditorGUI.EndChangeCheck())
			{
				EditorApplication.MarkSceneDirty();
			}
		}

		//call destroy if we have deleted the model
		public void OnDestroy()
		{
			if(!this.target)
			{
				ModelBase obj = (ModelBase)this.target;
				this.DisposeAndDestroy(obj);
			}
		}

		private List<ViewBaseNonGeneric> m_views;
	}
}


