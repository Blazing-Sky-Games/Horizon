using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[InlineData]
public class ResourceReference<ResourceType> : Data
	where ResourceType : UnityEngine.Object
{
	[ResourcePath]
	public string ResourcePath;

	public readonly Type ResType = typeof(ResourceType);

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




