using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// hmmm ... there seams to be an awful lot of logic in this ui class ... should refactor
// lol i love how this attack button is actually the attack button and the pass button
public class AttackButton : MonoBehaviour 
{
	// yay a bunch a variables
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

	// are we currently waiting for the player to select a unit
	public bool Selecting
	{
		get
		{
			return m_selecting;
		}
	}

	// cancle an attack
	public void CancelAttack()
	{
		targetedUnit = null;
		m_selecting = false;
	}

	// select a unit to attack
	public void TargetUnit(HorizonUnitModel unit)
	{
		targetedUnit = unit;
		m_selecting = false;
	}

	public IEnumerator SelectTargetForSelectedUnitRoutine()
	{
		// if there is no selected unit, this is a bug, so dont do anything
		if(m_gridModel.SelectedUnit == null) yield break;

		// if we are already selecting and we hit he button, cancle the attack
		if(m_selecting)
		{
			CancelAttack();
			yield break;
		}

		if(OnTargetBegin != null) OnTargetBegin(); // we started targeting

		m_selecting = true;

		while(m_selecting)
		{
			yield return null; // wait for the user to select a target
		}

		/// THIS CODE IS SUPER BUGGY RIGHT NOW
		// fix it so that OnTargetEnd can be called after the attack
		if(OnTargetEnd != null) OnTargetEnd();

		// change this to yielding an attack corutine, so it can wait for the animation to finish
		if(targetedUnit != null) m_gridModel.SelectedUnit.Attack(targetedUnit);
	}

	// handler for the attack button
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
		//ugh, this should be replaced with hooking up to some event
		// update button visibility
		gameObject.transform.GetChild(0).gameObject.SetActive(m_gridModel.SelectedUnit != null && m_gridModel.SelectedUnit.unitType == UnitType.Character && m_gridModel.SelectedUnit == m_gridModel.ActiveUnit && m_gridModel.SelectedUnit.hasAttacked == false);
		gameObject.transform.GetChild(1).gameObject.SetActive(m_gridModel.SelectedUnit != null && m_gridModel.SelectedUnit.unitType == UnitType.Character && m_gridModel.SelectedUnit == m_gridModel.ActiveUnit);

		buttonLabel.text = m_selecting ? "Cancle Attack" : "Attack"; // update button text
	}

	// on clik event for the pass button
	public void pass()
	{
		m_gridModel.ActiveUnit.PassTurn();
	}

}
