using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class HPLabel : MonoBehaviour 
{
	private RectTransform m_rectTransform;
	private Text m_label;

	private int m_maxHP;
	private int m_currentHP;

	void Awake()
	{
		m_rectTransform = gameObject.GetComponent<RectTransform>();
		m_label = gameObject.GetComponentInChildren<Text>();
	}

	public RectTransform rectTransform
	{
		get
		{
			return m_rectTransform;
		}
	}

	public int MaxHP
	{
		get
		{
			return m_maxHP;
		}
		set
		{
			m_maxHP = value;
			CurrentHP = (int)Mathf.Clamp(CurrentHP,0,m_maxHP);
			updateLabel();
		}
	}

	public int CurrentHP
	{
		get
		{
			return m_currentHP;
		}
		set
		{
			m_currentHP = (int)Mathf.Clamp(value,0,m_maxHP);
			updateLabel();
		}
	}

	private void updateLabel()
	{
		m_label.text = m_currentHP + "/" + m_maxHP;
	}
}
