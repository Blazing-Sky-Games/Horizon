/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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