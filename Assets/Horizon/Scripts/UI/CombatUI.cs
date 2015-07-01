using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

// handles creationg of ui elements, and provides accses to uniqe ui elements
public class CombatUI : MonoBehaviour 
{
	public GameObject HPLabelPrefab; // set this prefab in the editor to control what gets created when the game requests a hplabel
	public GameObject UnitSummaryPrefab; // set this prefab in the editor

	public HorizonGridModel GridModel; // set this in the editor
	public AttackButton attackButton; // set this in the editor
	public TurnOrderDisplay turnOrderDiaplsy;// set this in the editor

	private static CombatUI m_instance;

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
		//init the attack buttons model. ugh this is hacky
		attackButton.SetGridModel(GridModel);
	}

	public static CombatUI Instance
	{
		get
		{
			return m_instance;
		}
	}

	//creat a new hp label
	public static HPLabel NewHPLabel()
	{
		GameObject label = Instantiate(m_instance.HPLabelPrefab);
		HPLabel HPScript = label.GetComponent<HPLabel>();
		if(HPScript == null) throw new InvalidOperationException("HPLabelPrefab must have a HPLabel component attached");

		HPScript.rectTransform.SetParent(m_instance.transform.FindChild("HPLabels"),false);
		return HPScript;
	}

	//creat a new unit summary
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
