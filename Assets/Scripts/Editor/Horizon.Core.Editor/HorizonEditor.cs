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
	public class HorizonEditor : UnityEditor.Editor
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

		private void DisplayObjectThroughReflection(UnityEngine.Object obj)
		{
			//if(foldOutSets.ContainsKey(obj) == false) foldOutSets[obj] = new foldOutSet();

			//fields
			//if(obj.GetType().GetFields().Where(x=>x.FieldType != typeof(EventName)).Count() != 0 && (foldOutSets[obj].fields = EditorGUILayout.Foldout(foldOutSets[obj].fields,"Fields")))
			//{
				//using(new Indent())
				//{
					foreach(FieldInfo field in obj.GetType().GetFields())
					{
						HorizonEditorUtility.DisplayMemberValue(field,obj);
					}
				//}
			//}
			
			//properties
			var props =
				obj
				.GetType()
				.GetProperties()
				.Where(
					x => x.DeclaringType != typeof(UnityEngine.Object) 
					&& x.DeclaringType != typeof(Component) 
					&& x.DeclaringType != typeof(Behaviour) 
					&& x.DeclaringType != typeof(MonoBehaviour)
				);
			//if (props.Count() != 0 && (foldOutSets[obj].props = EditorGUILayout.Foldout(foldOutSets[obj].props,"Properties")))
			//{
				//using(new Indent())
				//{
					foreach(PropertyInfo property in props)
					{
						if(property.CanRead && property.CanWrite)
						{
							HorizonEditorUtility.DisplayMemberValue(property,obj);
						}
					}
				//}
			//}
			
			//funtions
			//todo
			
			//events
			//todo
		}

		// show all properties
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			GUIStyle boldAndItalicStyle = new GUIStyle();
			boldAndItalicStyle.fontStyle = FontStyle.BoldAndItalic;

			GUILayout.Label("Model",boldAndItalicStyle);

			DisplayObjectThroughReflection(target);

			Splitter();
		
			GUILayout.Label("Views",boldAndItalicStyle);

			List<ViewBaseNonGeneric> views = (List<ViewBaseNonGeneric>)(typeof(ModelBase).GetField("m_views", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target));
			foreach(var view in views)
			{
				if(view == null)
				{
					EditorGUILayout.LabelField("view","null");
					continue;
				}
			
				EditorGUILayout.InspectorTitlebar(true,view);
				
				DisplayObjectThroughReflection(view);
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

		//private bool showSubscribers = false;
		//private Dictionary<UnityEngine.Object, foldOutSet> foldOutSets = new Dictionary<UnityEngine.Object, foldOutSet>();
	}
}


