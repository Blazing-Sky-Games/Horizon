using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

// hmmm ... there seams to be an awful lot of logic in this ui class ... should refactor
public class AttackButton : MonoBehaviour 
{
	private bool m_selecting = false;
	private HorizonUnitModel targetedUnit = null;

	private Text buttonLabel;
	private Button button;

	private HorizonGridModel m_gridModel;

	public event Action OnTargetBegin;
	public event Action OnTargetEnd;

	public void SetGridModel(HorizonGridModel model)
	{
		m_gridModel = model;
	}

	public bool Selecting
	{
		get
		{
			return m_selecting;
		}
	}

	public void CancelAttack()
	{
		targetedUnit = null;
		m_selecting = false;
	}

	public void TargetUnit(HorizonUnitModel unit)
	{
		targetedUnit = unit;
		m_selecting = false;
	}

	public IEnumerator SelectTargetForSelectedUnitRoutine()
	{
		if(m_gridModel.SelectedUnit == null) yield break;

		if(m_selecting)
		{
			CancelAttack();
			yield break;
		}

		if(OnTargetBegin != null) OnTargetBegin();

		m_selecting = true;

		while(m_selecting)
		{
			yield return null;
		}

		if(OnTargetEnd != null) OnTargetEnd();

		if(targetedUnit != null) m_gridModel.SelectedUnit.Attack(targetedUnit);
	}

	public void SelectTargetForSelectedUnit()
	{
		StartCoroutine(SelectTargetForSelectedUnitRoutine());
	}

	void Awake()
	{
		buttonLabel = gameObject.GetComponentInChildren<Text>();
	}

	void Update()
	{
		gameObject.transform.GetChild(0).gameObject.SetActive(m_gridModel.SelectedUnit != null && m_gridModel.SelectedUnit == m_gridModel.ActiveUnit && m_gridModel.SelectedUnit.hasAttacked == false);
		gameObject.transform.GetChild(1).gameObject.SetActive(m_gridModel.SelectedUnit != null && m_gridModel.SelectedUnit == m_gridModel.ActiveUnit);

		buttonLabel.text = m_selecting ? "Cancle Attack" : "Attack";
	}

	public void pass()
	{
		m_gridModel.ActiveUnit.PassTurn();
	}

}
