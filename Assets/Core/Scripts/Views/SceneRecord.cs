using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

class SceneRecord
{
	public readonly string ContextType;
	public readonly string Path;
	public readonly string ContextGameObjectName;

	public SceneRecord(string contextType, string path, string contextGameObjectName)
	{
		ContextType = contextType;
		Path = path;
		ContextGameObjectName = contextGameObjectName;
	}
}
	
