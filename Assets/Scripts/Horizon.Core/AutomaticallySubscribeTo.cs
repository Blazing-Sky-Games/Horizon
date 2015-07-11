using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	//todo: create concept of manualsubscribers, and add functions hooked up to start, update, etc
	public class AutomaticallySubscribeTo<HorizonObjectType>  
		where HorizonObjectType:HorizonGameObjectBase
	{
		public AutomaticallySubscribeTo(HorizonObjectType HorizonObject)
		{
			this.HorizonObject = HorizonObject;
		}
		
		protected readonly HorizonObjectType HorizonObject;
	}
}


