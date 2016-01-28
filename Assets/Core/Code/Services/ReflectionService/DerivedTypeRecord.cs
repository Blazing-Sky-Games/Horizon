using System;
using System.Collections.Generic;
using System.Linq;

public class DerivedTypeRecord
{
	public DerivedTypeRecord(Type BaseType)
	{
		reflectionService = ServiceLocator.GetService<IReflectionService>();

		List<Type> derivedTypesList = reflectionService.AllTypes.Where(t => BaseType.IsAssignableFrom(t) && t.IsAbstract == false).ToList();

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

	private IReflectionService reflectionService;
}
