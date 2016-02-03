using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

public class Data : ScriptableObject
{
	public static IEnumerable<FieldInfo> GetAllFields(Type t)
	{
		if (t == null)
			return Enumerable.Empty<FieldInfo>();

		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Instance | BindingFlags.DeclaredOnly;
		return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
	}

	public ScriptableObject DeepCopy ()
	{
		Type dataType = GetType();

		FieldInfo[] fields = GetAllFields(dataType).ToArray();

		ScriptableObject Copy = ScriptableObject.CreateInstance(dataType);

		foreach(FieldInfo field in fields)
		{
			if(field.FieldType.IsSubclassOf(typeof(Data)))
			{
				field.SetValue(Copy, ((Data)field.GetValue(this)).DeepCopy());
			}
			else if(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type ElementType = field.FieldType.GetGenericArguments()[0];

				if(ElementType.IsSubclassOf(typeof(Data)))
				{
					IList listCopy = Activator.CreateInstance(field.FieldType) as IList;

					foreach(object obj in ((IEnumerable)field.GetValue(this)))
					{
						listCopy.Add(((Data)obj).DeepCopy());
					}

					field.SetValue(Copy, listCopy);
				}
				else
				{
					field.SetValue(Copy, field.GetValue(this));
				}
			}
			else
			{
				field.SetValue(Copy, field.GetValue(this));
			}
		}

		return Copy;
	}
}