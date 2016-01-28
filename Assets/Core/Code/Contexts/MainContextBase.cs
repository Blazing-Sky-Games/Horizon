using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public abstract class MainContextBase : Context
{
	public virtual IEnumerator WaitLoad (){ yield break;}

	public virtual void Update (){}

	public virtual void Unload (){}
}