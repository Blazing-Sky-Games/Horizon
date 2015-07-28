using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public class AutomaticallySubscribeToBase : ScriptableInit
	{
		public virtual void __setGameObject(object model){}
		public virtual object __getGameObject(){return null;}
	}
	
	public class AutomaticallySubscribeTo<HorizonObjectType> : AutomaticallySubscribeToBase
		where HorizonObjectType:HorizonGameObjectBase
	{
		protected override void Init()
		{
			base.Init();
			gameObject.WeakSubscribeToEvent(gameObject.StartEventName,(s,e) => Start());
			gameObject.WeakSubscribeToEvent(gameObject.UpdateEventName,(s,e) => Update());
			gameObject.WeakSubscribeToEvent(gameObject.LateUpdateEventName,(s,e) => LateUpdate());
		}

		protected virtual void Start(){}
		protected virtual void Update(){}
		protected virtual void LateUpdate(){}

		public override void __setGameObject(object model)
		{
			gameObject = (HorizonObjectType)model;
		}

		public override object __getGameObject ()
		{
			return gameObject;
		}

		[SerializeField]
		protected HorizonObjectType gameObject;
	}
}


