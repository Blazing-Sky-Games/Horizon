using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour 
{
	public static Globals instance
	{
		get
		{
			return m_instance;
		}
	}

	void Awake()
	{
		m_instance = this;
	}

	public Color onColor;
	public Color offColor;

	private static Globals m_instance;
}
