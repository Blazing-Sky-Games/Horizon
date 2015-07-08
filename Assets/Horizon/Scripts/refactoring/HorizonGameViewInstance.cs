using UnityEngine;
using System.Collections;
using System;
using Horizon.Models;

namespace Horizon.Views.Game
{
	public class HorizonGameViewInstance : HorizonBaseGameView<HorizonModelInstance>  
	{
		public HorizonGameViewInstance(HorizonModelInstance instance) :base(instance)
		{
			m_testChangedSubscription = Model.WeakSubscribe(() => Model.test, (sender,args) => HandleTestChange());
			m_drawSettings = new HorizonGLSettings(color: Color.green.SetAlpha(0.5f));
		}

		protected override void OnPostRender ()
		{
			base.OnPostRender ();

			HorizonGL.DrawLine(Model.transform.position , Model.transform.position + Vector3.forward,m_drawSettings);
		}

		private void HandleTestChange()
		{
			Debug.Log (Model.test);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			DisposeSubscription(ref m_testChangedSubscription);
		}

		private HorizonGLSettings m_drawSettings;
		private NotifyPropertyChangedEventSubscription m_testChangedSubscription;
	}
}


