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

using System;
using System.Collections.Generic;
using System.Linq;

public class DerivedTypeRecord
{
	public DerivedTypeRecord(Type BaseType)
	{
		List<Type> derivedTypesList = HorizonReflectionUtility.AllTypes.Where(t => isDerivedFromOrEqualTo(t,BaseType) && !t.IsAbstract && !t.ContainsGenericParameters).ToList();

		foreach(Type type in derivedTypesList)
		{
			string name = type.Name;

			derivedTypes.Add(new KeyValuePair<string,Type>(name,type));

			m_indexOfType[type] = derivedTypes.Count - 1;
			nameIndexLookup[name] = derivedTypes.Count - 1;
		}
	}

	bool isDerivedFromOrEqualTo(Type t, Type baseT)
	{
		bool eq = t == baseT;

		bool baseEq = false;
		if(t.BaseType != null)
		{
			if(t.BaseType.IsGenericType && baseT.IsGenericTypeDefinition)
			{
				baseEq = isDerivedFromOrEqualTo(t.BaseType.GetGenericTypeDefinition(), baseT);
			}
			else
			{
				baseEq = isDerivedFromOrEqualTo(t.BaseType, baseT);
			}
		}

		return eq || baseEq;
	}

	public IEnumerable<string> TypeNames
	{
		get
		{
			return derivedTypes.Select(x => x.Key);
		}
	}

	public IEnumerable<Type> Types
	{
		get
		{
			return derivedTypes.Select(x => x.Value);
		}
	}

	public int IndexOfType(Type t)
	{
		return m_indexOfType[t];
	}

	private readonly List<KeyValuePair<string,Type>> derivedTypes = new List<KeyValuePair<string, Type>>();
	private readonly Dictionary<Type, int> m_indexOfType = new Dictionary<Type, int>();
	private readonly Dictionary<string, int> nameIndexLookup = new Dictionary<string, int>();
}
