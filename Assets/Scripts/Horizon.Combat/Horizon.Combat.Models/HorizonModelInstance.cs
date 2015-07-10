using UnityEngine;
using System.Collections;
using Horizon.Core.Models;

namespace Horizon.Combat.Models
{
	public enum testenum{ONE,TWO,THREE}

	public class HorizonModelInstance : HorizonBaseModel 
	{
		public int PublicInt;

		public int test
		{
			get
			{
				return m_testSerialized;
			}
			set
			{
				SetPropertyFeild(ref m_testSerialized, value,() => test);
			}
		}

		public testenum enumTest
		{
			get
			{
				return m_enumSerialized;
			}
			set
			{
				SetPropertyFeild(ref m_enumSerialized, value,() => enumTest);
			}
		}

		public Bounds testBounds{get;set;}
		public Color testColor{get;set;}

		//private AnimationCurve curve = new AnimationCurve();
		public AnimationCurve testCurve{get;set;}

		public double testDouble{get;set;}
		public float testFloat{get;set;}
		public LayerMask testLayerMask{get;set;}
		public long testLong{get;set;}
		public string testString{get;set;}
		public bool testBool{get;set;}
		public Vector2 testVector2{get;set;}
		public Vector3 testVector3{get;set;}
		public Vector4 testVector4{get;set;}
		public UnityEngine.Object testObject{get;set;}

		[SerializeField]
		private int m_testSerialized = 0;
		[SerializeField]
		private testenum m_enumSerialized;
	}
}
