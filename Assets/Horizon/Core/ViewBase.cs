using System;
using UnityEngine;
using Horizon.Core;
using Horizon.Core.WeakSubscription;

namespace Horizon.Core
{
	//non generic base class of views
	//use to store generic views
	public abstract class ViewBaseNonGeneric : ScriptableObject , IDisposable
	{
		//used during initilization
		public virtual void SetModel(ModelBase model){}
		public virtual ModelBase GetModel(){return null;}

		public virtual void OnSceneGUI(){}

		//call when destroying a view
		public virtual void Dispose (){}
	}
	
	// the base class of all generic views
	// derive from ViewBase<someModelType> to automatically attach that view to all instances of that model
	public abstract class ViewBase<ModelType> : ViewBaseNonGeneric
		where ModelType:ModelBase
	{
		public override void SetModel(ModelBase model)
		{
			this.model = (ModelType)model;
			//init once we set the model
			Init();
		}
		
		public override ModelBase GetModel ()
		{
			return model;
		}

		// override this function to hook up to other model properties
		// called whenever the object is loaded, in or out of gameplay
		protected virtual void Init()
		{
			//hook up to events in the model
			model.WeakSubscribeToEvent(model.StartEventName,(s,e) => Start());
			model.WeakSubscribeToEvent(model.UpdateEventName,(s,e) => Update());
			model.WeakSubscribeToEvent(model.LateUpdateEventName,(s,e) => LateUpdate());
		}

		//override these in a derived view
		protected virtual void Start(){}
		protected virtual void Update(){}
		protected virtual void LateUpdate(){}

		[SerializeField]
		protected ModelType model;
	}
}


