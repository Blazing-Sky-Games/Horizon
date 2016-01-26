using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections;

namespace Core.Scripts.Contexts
{
	public abstract class MainContextBase : Context
	{
		public IEnumerator Load()
		{
			RegisterCoreServices();
			InstatiateCoreServices();
			yield return new Routine(Launch());
		}

		public abstract void Update();

		public void UnLoad()
		{
			RemoveCoreServices();
		}

		protected abstract void RegisterCoreServices ();

		protected abstract void InstatiateCoreServices ();

		protected abstract void Launch ();

		protected abstract void RemoveCoreServices ();
	}
}

