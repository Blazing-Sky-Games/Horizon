/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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