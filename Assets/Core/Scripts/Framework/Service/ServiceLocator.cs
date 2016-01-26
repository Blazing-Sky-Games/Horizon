using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ServiceLocator 
{	
	public static void RegisterService<ServiceType, ConreateType> (bool lazy = true) 
		where ServiceType : IService
		where ConreateType : ServiceType
	{
		ServiceType service = lazy ? null : (ServiceType)Activator.CreateInstance(typeof(ConreateType));;
		services[typeof(ServiceType)] = () =>
		{
			if(service == null)
				service = (ServiceType)Activator.CreateInstance(typeof(ConreateType));

			return service;
		};
	}

	public static ServiceType GetServiceReference<ServiceType>()
		where ServiceType : IService
	{
		if(!services.ContainsKey(typeof(ServiceType)))
			throw new InvalidOperationException("service of type " + typeof(ServiceType).Name + " not registered");

		return (ServiceType)services[typeof(ServiceType)]();
	}

	public static void RemoveService<ServiceType>()
		where ServiceType : IService
	{
		services.Remove(typeof(ServiceType));
	}

	private static Dictionary<Type, Func<IService>> services = new Dictionary<Type, Func<IService>>();
}
