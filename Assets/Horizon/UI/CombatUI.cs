using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour 
{
	public GameObject HPLabelPrefab;
	public HorizonGridModel GridModel;
	public AttackButton attackButton;

	private static CombatUI m_instance;

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
		attackButton.SetGridModel(GridModel);
	}

	public static CombatUI Instance
	{
		get
		{
			return m_instance;
		}
	}

	public static HPLabel NewHPLabel()
	{
		GameObject label = Instantiate(m_instance.HPLabelPrefab);
		HPLabel HPScript = label.GetComponent<HPLabel>();
		if(HPScript == null) throw new InvalidOperationException("LabelPrefab must have a HPLabel component attached");

		HPScript.rectTransform.SetParent(m_instance.transform,false);
		return HPScript;
	}
}
