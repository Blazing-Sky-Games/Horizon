using System;
using UnityEditor;
using Core.Scripts.Contexts;

namespace Core.Code.Editor
{
	[InitializeOnLoad]
	public class HorizonEditorAplication
	{
		static HorizonEditorAplication()
		{
			ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
		}
	}
}

