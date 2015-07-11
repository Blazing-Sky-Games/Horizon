using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public class AutomaticallySubscribeTo<HorizonObjectType> : IDisposable  
		where HorizonObjectType:HorizonGameObjectBase
	{
		public virtual void Dispose(){}

		public AutomaticallySubscribeTo(HorizonObjectType HorizonObject)
		{
			this.HorizonObject = HorizonObject;
		}
		
		protected readonly HorizonObjectType HorizonObject;
	}
}


