using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ResourceService : Service, IResourceService
{
	private TextAsset ResourceList;
	private List<ResourceId> ResourceIds = new List<ResourceId>();

	public override void LoadService ()
	{
		base.LoadService();
		ResourceList = Resources.Load<TextAsset>("ResourceList.txt");
		loadResources();
	}

	public IEnumerable<ResourceId> GetResourceIdsOfType<resType>()
	{
		return ResourceIds.Where(x => x.ResType == typeof(resType));
	}

	private void loadResources()
	{
		StringReader sr = new StringReader(ResourceList.text);

		string line = "";
		while((line = sr.ReadLine()) != null)
		{
			string[] splitLine = line.Split();
			string path = splitLine[0];
			string assemblyQualiyfiedName = splitLine[1];

			ResourceIds.Add(new ResourceId());
		}
	}
}


