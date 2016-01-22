using System;

public class WeakReference<objType>
{
	public WeakReference(objType obj)
	{
		objref = new WeakReference(obj);
	}

	public bool NullReference
	{
		get
		{
			return !objref.IsAlive;
		}
	}

	public objType Dereference()
	{
		if(NullReference)
			throw new NullReferenceException("this weak reference is null");

		return (objType)objref.Target;
	}

	public WeakReference objref;
}


