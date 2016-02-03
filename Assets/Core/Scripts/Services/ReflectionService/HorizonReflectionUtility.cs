using System.Collections.Generic;
using System;
using System.Linq;

public static class HorizonReflectionUtility
{
	public static IEnumerable<Type> AllTypes
	{
		get
		{
			if(m_allTypes == null)
			{
				m_allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();
			}

			return m_allTypes;
		}
	}

	public static DerivedTypeRecord GetDerivedTypeRecord(Type baseType)
	{
		if (!DerivedTypeDictionary.ContainsKey (baseType))
			DerivedTypeDictionary [baseType] = new DerivedTypeRecord (baseType);

		return DerivedTypeDictionary[baseType];
	}

	private static Dictionary<Type, DerivedTypeRecord> DerivedTypeDictionary = new Dictionary<Type, DerivedTypeRecord>();

	private static List<Type> m_allTypes;
}


