using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class Service : IService
{
	public virtual IEnumerator WaitLoadService ()
	{
		yield break;
	}
	public virtual void UnloadService ()
	{
	}

}
