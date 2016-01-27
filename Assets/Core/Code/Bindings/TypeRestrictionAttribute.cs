using System;
using Slash.Unity.DataBind.Core.Data;

namespace Core.Scripts.Contexts
{
	public class TypeRestrictionAttribute : Attribute
	{
		public readonly Type BaseType;

		public TypeRestrictionAttribute(Type basetype)
		{
			BaseType = basetype;
		}
	}

}

