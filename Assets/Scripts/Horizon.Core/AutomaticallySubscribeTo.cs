using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public class AutomaticallySubscribeToBase : ScriptableObject
	{
		public virtual void __SetModel(object model){}
	}
	
	public class AutomaticallySubscribeTo<HorizonObjectType> : AutomaticallySubscribeToBase
		where HorizonObjectType:HorizonGameObjectBase
	{
		public AutomaticallySubscribeTo()
		{
			Initilizer.CallOnInit(Init);
		}

		protected virtual void Init()
		{
			HorizonObject.WeakSubscribeToEvent(HorizonObject.StartEventName,(s,e) => Start());
			HorizonObject.WeakSubscribeToEvent(HorizonObject.UpdateEventName,(s,e) => Update());
			HorizonObject.WeakSubscribeToEvent(HorizonObject.LateUpdateEventName,(s,e) => LateUpdate());
		}

		protected virtual void Start(){}
		protected virtual void Update(){}
		protected virtual void LateUpdate(){}

		public override void __SetModel(object model)
		{
			HorizonObject = (HorizonObjectType)model;
		}

		[SerializeField]
		protected HorizonObjectType HorizonObject;
	}
}


