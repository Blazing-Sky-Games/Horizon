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
