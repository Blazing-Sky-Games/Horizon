using UnityEditor;
using UnityEditor.ProjectWindowCallback;

public class ScriptableObjectWindowEndNameEditAction : EndNameEditAction
{
	public override void Action(int instanceId, string pathName, string resourceFile)
	{
		AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
	}
}
