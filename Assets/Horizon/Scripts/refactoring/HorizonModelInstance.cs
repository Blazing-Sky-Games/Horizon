using UnityEngine;
using System.Collections;

namespace Horizon.Models
{
	public enum testenum{ONE,TWO,THREE}

	public class HorizonModelInstance : HorizonBaseModel 
	{
		// should not be visible
		public int publicint;

		public int test
		{
			get
			{
				return m_test;
			}
			set
			{
				SetPropertyFeild(ref m_test, value,() => test);
			}
		}

		public testenum enumtest
		{
			get
			{
				return m_enum;
			}
			set
			{
				m_enum = value;
			}
		}

		[SerializeField]
		private int m_test = 0;
		[SerializeField]
		private testenum m_enum;
	}
}
