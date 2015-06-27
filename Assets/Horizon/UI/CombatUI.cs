using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour 
{
	public GameObject HPLabelPrefab;
	public GameObject UnitSummaryPrefab;

	public HorizonGridModel GridModel;
	public AttackButton attackButton;
	public TurnOrderDisplay turnOrderDiaplsy;

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
		if(HPScript == null) throw new InvalidOperationException("HPLabelPrefab must have a HPLabel component attached");

		HPScript.rectTransform.SetParent(m_instance.transform.FindChild("HPLabels"),false);
		return HPScript;
	}

	public static UnitSummary NewUnitSummary()
	{
		GameObject summary = Instantiate(m_instance.UnitSummaryPrefab);
		UnitSummary summaryScript = summary.GetComponent<UnitSummary>();

		if(summaryScript == null) throw new InvalidOperationException("UnitSummaryPrefab must have a UnitSummary component attached");

		summaryScript.rectTransform.SetParent(m_instance.transform,false);
		summaryScript.SetGridModel(m_instance.GridModel);
		return summaryScript;
	}
}
