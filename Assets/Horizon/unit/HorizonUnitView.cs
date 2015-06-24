using UnityEngine;
using System.Collections;

public class HorizonUnitView : MonoBehaviour 
{
	private float outlineSize = 0.05f;

	public float OutlineSize
	{
		get
		{
			return outlineSize;
		}
		set
		{
			outlineSize = Mathf.Clamp(value,0,0.1f);
			gameObject.GetComponentInChildren<MeshRenderer>().material.SetFloat("_Outline",outlineSize);
		}
	}

	public int OutlinePercentage
	{
		get
		{
			return (int)OutlineSize/100;
		}
		set
		{
			OutlineSize = 0.001f * value;
		}
	}
}
