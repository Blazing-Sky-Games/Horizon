using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ServiceUtility 
{
	public static void Init()
	{
		SceneUtility.SceneLoaded.AddAction(OnSceneLoad);
		SceneUtility.SceneUnloaded.AddAction(OnSceneUnload);

		foreach(Type type in ReflectionUtility.GetTaggedTypes<ServiceAttribute>())
		{
			ServiceAttribute serviceAttrib = (ServiceAttribute)type.GetCustomAttributes(typeof(ServiceAttribute), true).First();

			if(serviceAttrib.Scene == SceneType.Core)
			{
				IService service = (IService)Activator.CreateInstance(type);
				services[serviceAttrib.InterfaceType] = service;
			}
		}
	}

	private static void OnSceneLoad(SceneType scene)
	{
		foreach(Type type in ReflectionUtility.GetTaggedTypes<ServiceAttribute>())
		{
			ServiceAttribute serviceAttrib = (ServiceAttribute)type.GetCustomAttributes(typeof(ServiceAttribute), true).First();

			if(serviceAttrib.Scene == scene)
			{
				IService service = (IService)Activator.CreateInstance(type);
				services[serviceAttrib.InterfaceType] = service;
			}
		}
	}

	private static void OnSceneUnload(SceneType scene)
	{
		foreach(Type type in ReflectionUtility.GetTaggedTypes<ServiceAttribute>())
		{
			ServiceAttribute serviceAttrib = (ServiceAttribute)type.GetCustomAttributes(typeof(ServiceAttribute), true).First();

			if(serviceAttrib.Scene == scene)
			{
				services[serviceAttrib.InterfaceType].UnloadService();
				services.Remove(serviceAttrib.InterfaceType);
			}
		}
	}

	public static void RegisterService<ServiceType, ConreateType> () 
		where ServiceType : IService
		where ConreateType : ServiceType
	{
		services[typeof(ServiceType)] = (ServiceType)Activator.CreateInstance(typeof(ConreateType));
	}

	public static WeakReference<ServiceType> GetServiceReference<ServiceType>()
		where ServiceType : IService
	{
		if(!services.ContainsKey(typeof(ServiceType)))
			throw new InvalidOperationException("service of type " + typeof(ServiceType).Name + " not registered");

		return new WeakReference<ServiceType>((ServiceType)services[typeof(ServiceType)]);
	}

	public static void RemoveService<ServiceType>()
		where ServiceType : IService
	{
		services.Remove(typeof(ServiceType));
	}

	private static Dictionary<Type, IService> services = new Dictionary<Type, IService>();
}
