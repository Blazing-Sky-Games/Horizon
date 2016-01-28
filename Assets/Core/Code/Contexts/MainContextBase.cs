using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public abstract class MainContextBase : Context
{
	public IEnumerator Load ()
	{
		RegisterCoreServices();
		InstatiateCoreServices();
		yield return new Routine(Launch());
	}

	public abstract void Update ();

	public void UnLoad ()
	{
		RemoveServiceReferences();
		RemoveCoreServices();
	}

	protected abstract void RegisterCoreServices ();

	protected abstract void InstatiateCoreServices ();

	protected abstract IEnumerator Launch ();

	protected abstract void RemoveServiceReferences ();

	protected abstract void RemoveCoreServices ();
}