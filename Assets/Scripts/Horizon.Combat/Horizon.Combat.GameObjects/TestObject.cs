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

		public Bounds testBounds{get;set;}
		public Color testColor{get;set;}

		private AnimationCurve curve = new AnimationCurve();
		public AnimationCurve testCurve{get{return curve;}set{curve = value;}}

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
		private TestEnum m_enumSerialized;
	}
}
