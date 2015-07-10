using UnityEngine;
using System.Collections;
using System;
using Horizon.Core.Objects;
using Horizon.Core.Views.GameViews;
using Horizon.Combat.Models;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;
using Horizon.Core.ExtensionMethods;

namespace Horizon.Combat.Views.GameViews
{
	public class HorizonGameViewInstance : HorizonBaseGameView<HorizonModelInstance>  
	{
		public HorizonGameViewInstance(HorizonModelInstance instance) : base(instance)
		{
			Model.WeakSubscribe(() => Model.test, (sender,args) => HandleTestChange());

			m_line = new GLLine();
			m_line.StartPoint = new Vector3(0,0,0); 
			m_line.EndPoint = new Vector3(0,0,1);
			m_line.Settings = new GLSettings(color:Color.green.SetAlpha(0.5f));
		}

		private void HandleTestChange()
		{
			Debug.Log (Model.test);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			m_line.Dispose();
		}

		private GLLine m_line;
	}
}


