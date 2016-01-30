using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class Service : IService
{
	public virtual IEnumerator LoadService ()
	{
		yield break;
	}
	public virtual void UnloadService ()
	{
	}

}
