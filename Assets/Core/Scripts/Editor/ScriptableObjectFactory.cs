using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//A helper class for instantiating ScriptableObjects in the editor.
public class ScriptableObjectFactory
{
	[MenuItem("Assets/Create/ScriptableObject")]
	public static void CreateScriptableObject()
	{
		var assembly = GetAssembly();
		
		// Get all classes derived from ScriptableObject
		var allScriptableObjects = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ScriptableObject)) && t.GetCustomAttributes(typeof(Inline),true).Count() == 0).ToArray();
		
		// Show the selection window.
		ScriptableObjectWindow.Init(allScriptableObjects);
	}

	// Returns the assembly that contains the script code for this project (currently hard coded)
	private static Assembly GetAssembly()
	{
		return Assembly.Load(new AssemblyName("Assembly-CSharp"));
	}
}