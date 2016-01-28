using System.Collections.Generic;
using System;
using System.Linq;

public class ReflectionService : IReflectionService
{
	#region IService implementation

	public void LoadService ()
	{
		throw new NotImplementedException();
	}

	public void UnloadService ()
	{
		throw new NotImplementedException();
	}

	#endregion

	public IEnumerable<Type> AllTypes
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

	public IEnumerable<Type> GetTaggedTypes<AttributeType>()
		where AttributeType : Attribute
	{
		if(!TaggedTypeDictionary.ContainsKey(typeof(AttributeType)))
		{
			TaggedTypeDictionary[typeof(AttributeType)] = AllTypes.Where(t => t.GetCustomAttributes(typeof(AttributeType), true).Count() > 0).ToList();
		}

		return TaggedTypeDictionary[typeof(AttributeType)];
	}

	public DerivedTypeRecord GetDerivedTypes(Type baseType)
	{
		if (!DerivedTypeDictionary.ContainsKey (baseType))
			DerivedTypeDictionary [baseType] = new DerivedTypeRecord (baseType);

		return DerivedTypeDictionary[baseType];
	}



	private Dictionary<Type, DerivedTypeRecord> DerivedTypeDictionary = new Dictionary<Type, DerivedTypeRecord>();
	private Dictionary<Type, List<Type>> TaggedTypeDictionary = new Dictionary<Type, List<Type>>();

	private List<Type> m_allTypes;
}


