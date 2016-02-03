using System.Reflection;
using UnityEditor;
using System.Linq;

public class DataFactory
{
	[MenuItem("Assets/Create/Data")]
	public static void CreateData()
	{
		var assembly = GetAssembly();
		
		// Get all classes derived from Data
		var allDataTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Data)) && !t.IsSubclassOf(typeof(PolymorphicSerializable))).ToArray();
		
		// Show the selection window.
		DataWindow.Init(allDataTypes);
	}

	// Returns the assembly that contains the script code for this project (currently hard coded)
	private static Assembly GetAssembly()
	{
		return Assembly.Load(new AssemblyName("Assembly-CSharp"));
	}
}