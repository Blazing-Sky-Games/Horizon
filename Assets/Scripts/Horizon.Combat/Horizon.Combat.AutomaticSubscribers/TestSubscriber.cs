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
		[SerializeField]
		private Color lineColor;

		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				m_line.Settings = new GLSettings(color:value);
				lineColor = value;
			}
		}

		protected override void Init ()
		{
			base.Init ();
			HorizonObject.WeakSubscribeToProperty(() => HorizonObject.testInt, (sender,args) => HandleTestChange());

			m_line = new GLLine(HorizonObject);
			m_line.StartPoint = new Vector3(0,0,0); 
			m_line.EndPoint = new Vector3(0,0,1);
			m_line.Settings = new GLSettings(color:lineColor);
		}

		private void HandleTestChange()
		{
			Debug.Log (HorizonObject.testInt);
		}

		private GLLine m_line;
	}
}


