using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	public abstract class ViewBaseNonGeneric : ScriptableObject , IDisposable
	{
		public virtual void SetModel(ModelBase model){}
		public virtual ModelBase GetModel(){return null;}

		#region IDisposable implementation

		public virtual void Dispose (){}

		#endregion
	}
	
	public abstract class ViewBase<ModelType> : ViewBaseNonGeneric
		where ModelType:ModelBase
	{
		public override void SetModel(ModelBase model)
		{
			this.model = (ModelType)model;
			Init();
		}
		
		public override ModelBase GetModel ()
		{
			return model;
		}

		protected virtual void Init()
		{
			model.WeakSubscribeToEvent(model.StartEventName,(s,e) => Start());
			model.WeakSubscribeToEvent(model.UpdateEventName,(s,e) => Update());
			model.WeakSubscribeToEvent(model.LateUpdateEventName,(s,e) => LateUpdate());
		}

		protected virtual void Start(){}
		protected virtual void Update(){}
		protected virtual void LateUpdate(){}

		[SerializeField]
		protected ModelType model;
	}
}


