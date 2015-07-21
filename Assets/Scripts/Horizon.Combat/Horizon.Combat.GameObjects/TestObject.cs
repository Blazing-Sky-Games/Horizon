using UnityEngine;
using System.Collections;
using Horizon.Core;

namespace Horizon.Combat.GameObjects
{
	public enum TestEnum{ONE,TWO,THREE}

	public class TestObject : HorizonGameObjectBase 
	{
		public int testInt
		{
			get
			{
				return m_testSerialized;
			}
			set
			{
				SetPropertyFeild(ref m_testSerialized, value,() => testInt);
			}
		}

		public TestEnum testEnum
		{
			get
			{
				return m_enumSerialized;
			}
			set
			{
				SetPropertyFeild(ref m_enumSerialized, value,() => testEnum);
			}
		}

		[SerializeField]
		private int m_testSerialized = 0;
		[SerializeField]
		private TestEnum m_enumSerialized;
	}
}
