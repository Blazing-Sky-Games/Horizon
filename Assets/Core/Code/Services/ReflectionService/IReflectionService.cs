using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections.Generic;

namespace Core.Scripts.Contexts
{
	public interface IReflectionService : IService
	{
		IEnumerable<Type> AllTypes{ get;}
		DerivedTypeRecord GetDerivedTypes (Type baseType);
	}
}

