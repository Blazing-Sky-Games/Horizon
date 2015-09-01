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
	//TODO: support multiobject editing. use showmixedvalue
	[CustomEditor(typeof( ModelBase ), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class ModelInspector : UnityEditor.Editor
	{
		//todo make it so you can close the foldouts
		//private bool showFeilds = true;
		//private bool showProps = false;

		private class foldOutSet
		{
			public bool fields;
			public bool props;

			public foldOutSet()
			{
				fields = false;
				props = false;
			}
		}

		public class Indent : IDisposable
		{
			public Indent()
			{
				EditorGUI.indentLevel++;
			}

			public void Dispose()
			{
				EditorGUI.indentLevel--;
			}
		}

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

		private void DisplayObjectsThroughReflection(UnityEngine.Object[] objs)
		{
			foreach(FieldInfo field in objs[0].GetType().GetFields().Where(x => x.GetCustomAttributes(typeof(HideInInspector),true).Count() == 0))
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
				)
				.Where(x => x.GetCustomAttributes(typeof(HideInInspector),true).Count() == 0);

				foreach(PropertyInfo property in props)
				{
					if(property.CanRead && property.CanWrite)
					{
						ModelInspectorUtility.DisplayMemberValue(property,objs);
					}
				}
		}

		private void OnSceneGUI()
		{
			if(views == null)
				views = (List<ViewBaseNonGeneric>)(typeof(ModelBase).GetField("m_views", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target));

			//Handles.ScaleValueHandle(0,Vector3.zero,Quaternion.identity,30,Handles.ArrowCap,1);

			foreach(var view in views)
			{
				view.OnSceneGUI();
			}
		}

		// show all properties
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			GUIStyle boldAndItalicStyle = new GUIStyle();
			boldAndItalicStyle.fontStyle = FontStyle.BoldAndItalic;

			GUILayout.Label("Model",boldAndItalicStyle);

			DisplayObjectsThroughReflection(targets);

			Splitter();
		
			GUILayout.Label("Views",boldAndItalicStyle);

			// stores the views of all selected models, based on type
			Dictionary<Type, List<ViewBaseNonGeneric>> viewLookup = new Dictionary<Type, List<ViewBaseNonGeneric>>();
			foreach(object obj in targets)
			{
				if(views == null)
					views = (List<ViewBaseNonGeneric>)(typeof(ModelBase).GetField("m_views", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj));

				foreach(ViewBaseNonGeneric view in views)
				{
					if(viewLookup.ContainsKey(view.GetType()) == false) viewLookup[view.GetType()] = new List<ViewBaseNonGeneric>(); 

					viewLookup[view.GetType()].Add(view);
				}
			}

			//foreach type of view on this type of model
			foreach(KeyValuePair<Type, List<ViewBaseNonGeneric>> entry in viewLookup)
			{
				EditorGUILayout.InspectorTitlebar(true,entry.Value[0]);
				
				DisplayObjectsThroughReflection(entry.Value.ToArray());
			}

			if(EditorGUI.EndChangeCheck())
			{
				EditorApplication.MarkSceneDirty();
			}
		}

		public void OnDestroy()
		{
			if(!this.target)
			{
				ModelBase obj = (ModelBase)this.target;
				this.DisposeAndDestroy(obj);
			}
		}

		List<ViewBaseNonGeneric> views;

		//private bool showSubscribers = false;
		//private Dictionary<UnityEngine.Object, foldOutSet> foldOutSets = new Dictionary<UnityEngine.Object, foldOutSet>();
	}
}


