using System;
using System.Collections.Generic;

public interface IResourceService : IService
{
	IEnumerable<ResourceId> GetResourceIdsOfType<resType>();
}



