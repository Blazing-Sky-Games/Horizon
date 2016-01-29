using System;
using System.Collections.Generic;

public class ResourceService : Service, IResourceService
{
	public const string ResourceListName = "ResourceList.txt";

	public IEnumerable<IResource> Resources
	{
		get
		{
			throw new NotImplementedException();
		}
	}
}




