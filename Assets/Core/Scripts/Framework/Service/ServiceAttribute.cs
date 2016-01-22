using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
	public ServiceAttribute(Type interfaceType, SceneType scene = SceneType.Core)
	{
		InterfaceType = interfaceType;
		Scene = scene;
	}

	public readonly Type InterfaceType;
	public readonly SceneType Scene;
}

