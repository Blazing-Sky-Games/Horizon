using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public interface IResoureReference
{
	Type ResType
	{
		get;
	}
}

[Serializable]
public class ResourceReference<ResourceType> : IResoureReference
	where ResourceType : UnityEngine.Object
{
	public string ResourcePath;

	public Type ResType
	{
		get{ return m_resType; }
	}

	private readonly Type m_resType = typeof(ResourceType);

	public ResourceType Asset
	{
		get
		{ 
			if(!m_loaded)
				throw new InvalidOperationException("this resource has not been loaded");

			return m_resource; 
		}
	}

	public IEnumerator WaitLoad()
	{
		ResourceRequest req = Resources.LoadAsync<ResourceType>(ResourcePath);
		while(!req.isDone)
		{
			yield return new WaitForNextUpdate();
		}

		m_loaded = true;
		m_resource = (ResourceType)req.asset;
	}

	bool m_loaded;
	ResourceType m_resource;
}




