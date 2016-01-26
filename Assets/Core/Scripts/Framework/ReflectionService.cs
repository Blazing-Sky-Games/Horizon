using System;
using System.Collections.Generic;
using System.Linq;

public static class ReflectionService
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

	public static IEnumerable<Type> GetTaggedTypes<AttributeType>()
		where AttributeType : Attribute
	{
		if(!TaggedTypeDictionary.ContainsKey(typeof(AttributeType)))
		{
			TaggedTypeDictionary[typeof(AttributeType)] = AllTypes.Where(t => t.GetCustomAttributes(typeof(AttributeType), true).Count() > 0).ToList();
		}

		return TaggedTypeDictionary[typeof(AttributeType)];
	}

	public static DerivedTypeRecord GetDerivedTypes(Type baseType)
	{
		if (!DerivedTypeDictionary.ContainsKey (baseType))
			DerivedTypeDictionary [baseType] = new DerivedTypeRecord (baseType);

		return DerivedTypeDictionary[baseType];
	}

	public class DerivedTypeRecord
	{
		public DerivedTypeRecord(Type BaseType)
		{
			List<Type> derivedTypesList = ReflectionService.AllTypes.Where(t => BaseType.IsAssignableFrom(t) && t.IsAbstract == false).ToList();

			foreach(Type type in derivedTypesList)
			{
				string name = type.Name;

				derivedTypes.Add(new KeyValuePair<string,Type>(name,type));

				typeIndexLookup[type] = derivedTypes.Count - 1;
				nameIndexLookup[name] = derivedTypes.Count - 1;
			}
		}

		public readonly List<KeyValuePair<string,Type>> derivedTypes = new List<KeyValuePair<string, Type>>();
		public readonly Dictionary<Type, int> typeIndexLookup = new Dictionary<Type, int>();
		public readonly Dictionary<string, int> nameIndexLookup = new Dictionary<string, int>();
	}

	private static Dictionary<Type, DerivedTypeRecord> DerivedTypeDictionary = new Dictionary<Type, DerivedTypeRecord>();
	private static Dictionary<Type, List<Type>> TaggedTypeDictionary = new Dictionary<Type, List<Type>>();

	private static List<Type> m_allTypes;
}


