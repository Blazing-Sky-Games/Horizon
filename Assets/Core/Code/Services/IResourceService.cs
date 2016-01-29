using System;
using System.Collections.Generic;

public interface IResourceService : IService
{
	IEnumerable<IResource> Resources{ get; }
}



