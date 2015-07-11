using System;
using System.Collections;
using UnityEngine;
using Horizon.Combat.GameObjects;
using Horizon.Core;
using Horizon.Core.ExtensionMethods;
using Horizon.Core.GL;
using Horizon.Core.WeakSubscription;

namespace Horizon.Combat.AutomaticSubscribers
{
	public class TestSubscriber : AutomaticallySubscribeTo<TestObject> 
	{
		public TestSubscriber(TestObject instance) : base(instance)
		{
			HorizonObject.WeakSubscribeToProperty(() => HorizonObject.testInt, (sender,args) => HandleTestChange());

			m_line = new GLLine();
			m_line.StartPoint = new Vector3(0,0,0); 
			m_line.EndPoint = new Vector3(0,0,1);
			m_line.Settings = new GLSettings(color:Color.green.SetAlpha(0.5f));
		}

		private void HandleTestChange()
		{
			Debug.Log (HorizonObject.testInt);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			m_line.Dispose();
		}

		private GLLine m_line;
	}
}


