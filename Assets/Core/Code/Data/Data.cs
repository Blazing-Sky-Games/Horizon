using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

//tag data type with this to have it display inline in inspector
//used to select an implimentation of an abstract data type
//this data type will not show up in the creat data window
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class InlineData : Attribute
{
}

//tag data type with this to control how it displays in create data menu
// for example "combat" will appear in a combat drop down
// "combat/logic" will appear in a drop down inside the dropdown combat
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DataCatagory : Attribute
{
	public readonly string Catagory;

	public DataCatagory(string catagory)
	{
		Catagory = catagory;
	}
}

public class Data : ScriptableObject
{
	public ScriptableObject DeepCopy ()
	{
		Type dataType = GetType();

		FieldInfo[] fields = dataType.GetFields();

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


