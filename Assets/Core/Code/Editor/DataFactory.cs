using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//A helper class for instantiating Data in the editor.
public class DataFactory
{
	[MenuItem("Assets/Create/Data")]
	public static void CreateData()
	{
		var assembly = GetAssembly();
		
		// Get all classes derived from Data
		var allDataTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Data)) && t.GetCustomAttributes(typeof(InlineData),true).Count() == 0).ToArray();
		
		// Show the selection window.
		DataWindow.Init(allDataTypes);
	}

	// Returns the assembly that contains the script code for this project (currently hard coded)
	private static Assembly GetAssembly()
	{
		return Assembly.Load(new AssemblyName("Assembly-CSharp"));
	}
}