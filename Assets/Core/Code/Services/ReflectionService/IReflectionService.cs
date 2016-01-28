using System;
using System.Collections.Generic;

public interface IReflectionService : IService
{
	IEnumerable<Type> AllTypes{ get; }

	DerivedTypeRecord GetDerivedTypes (Type baseType);
}


