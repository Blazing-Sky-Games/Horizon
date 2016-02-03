using System.Collections.Generic;
using Slash.Unity.DataBind.Core.Utils;
using UnityEditor;
using UnityEngine;
using Slash.Unity.DataBind.Editor.Utils;
using System.Linq;

[CustomPropertyDrawer(typeof(TargetPathAttribute))]
public class TargetPathPropertyDrawer : PropertyDrawer
{
	#region Constants

	private const float LineHeight = 16f;

	private const float LineSpacing = 2f;

	#endregion

	#region Fields

	private readonly Dictionary<string, bool> hasPropertyCustomPath = new Dictionary<string, bool>();

	#endregion

	#region Public Methods and Operators

	public bool HasTarget (SerializedProperty property)
	{
		var parentObj = property.serializedObject.targetObject;
		var parentType = parentObj.GetType();
		var targetBehaviorType = typeof(TargetBindingBase);
		return parentType.IsSubclassOf(targetBehaviorType) && targetBehaviorType.GetField("Target").GetValue(parentObj) != null;
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return LineHeight + (this.HasCustomPath(property.propertyPath) ? LineHeight + LineSpacing : 0);
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		var targetPathAttribute = this.attribute as TargetPathAttribute; 
		var pathDisplayName = targetPathAttribute != null
		                       && !string.IsNullOrEmpty(targetPathAttribute.PathDisplayName)
				? targetPathAttribute.PathDisplayName
				: "Path";

		if(HasTarget(property))
		{
			var targetObject = property.serializedObject.targetObject;
			var targetType = targetObject.GetType().GetField("Target").GetValue(targetObject).GetType();
			var hasCustomPath = this.HasCustomPath(property.propertyPath);
			property.stringValue = PathPopup(
				position,
				property.stringValue,
				ContextTypeCache.GetPaths(
					targetType,
					targetPathAttribute != null ? targetPathAttribute.Filter : ContextMemberFilter.All,
					targetPathAttribute != null ? targetPathAttribute.NodeType : null),
				pathDisplayName,
				ref hasCustomPath);
			this.hasPropertyCustomPath[property.propertyPath] = hasCustomPath;
		}
		else
		{
			var hasCustomPath = this.HasCustomPath(property.propertyPath);
			property.stringValue = PathPopup(
				position,
				property.stringValue,
				new List<string>(){ "None" },
				pathDisplayName,
				ref hasCustomPath);
			this.hasPropertyCustomPath[property.propertyPath] = hasCustomPath;
		}
	}

	#endregion

	#region Methods

	private static string ConvertPathToDisplayOption (string path)
	{
		return path.Replace('.', '/');
	}

	private bool HasCustomPath (string propertyPath)
	{
		bool hasCustomPath;
		this.hasPropertyCustomPath.TryGetValue(propertyPath, out hasCustomPath);
		return hasCustomPath;
	}

	private static string PathPopup (Rect position, string path, IList<string> paths, string pathDisplayName, ref bool customPath)
	{
		var selectedIndex = paths != null ? paths.IndexOf(path) : -1;
		if(selectedIndex < 0 || customPath)
		{
			// Select custom value.
			selectedIndex = 0;
			customPath = true;
		}
		else
		{
			// Custom option is prepended.
			++selectedIndex;
		}

		var displayedOptions = new List<GUIContent> { new GUIContent { text = "CUSTOM" } };
		if(paths != null)
		{
			displayedOptions.AddRange(
				paths.Select(existingPath => new GUIContent(ConvertPathToDisplayOption(existingPath))));
		}

		var newSelectedIndex = EditorGUI.Popup(
			                        new Rect(position.x, position.y, position.width, LineHeight),
			                        new GUIContent(pathDisplayName),
			                        selectedIndex,
			                        displayedOptions.ToArray());
		var newPath = path;
		if(newSelectedIndex != selectedIndex)
		{
			if(newSelectedIndex <= 0)
			{
				customPath = true;
			}
			else if(paths != null)
			{
				customPath = false;
				newPath = paths[newSelectedIndex - 1];
			}
		}

		if(customPath)
		{
			position.y += LineHeight + LineSpacing;
			newPath = EditorGUI.TextField(
				new Rect(position.x, position.y, position.width, LineHeight),
				new GUIContent("Custom Path"),
				newPath);
		}

		return newPath;
	}

	#endregion
}