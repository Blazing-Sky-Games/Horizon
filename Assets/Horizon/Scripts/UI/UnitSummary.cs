using UnityEngine;
using UnityEngine.UI;

using System.Collections;

//the little boxes in the turn order
public class UnitSummary : MonoBehaviour 
{
	private Image m_background;
	private Image m_portraite;

	public RectTransform rectTransform;

	private HorizonUnitGameView m_unit;
	private HorizonGridModel m_gridModel;

	Color Netural = new Color(0,0,0,0);
	Color Highlighted = Color.cyan * new Color(1,1,1,0.75f);
	Color Active = Color.red * new Color(1,1,1,0.5f);

	public void SetUnit(HorizonUnitGameView unit)
	{
		m_unit = unit;
		m_portraite.sprite = m_unit.Portrait; // update the portraite
	}

	public void SetGridModel(HorizonGridModel model)
	{
		m_gridModel = model;
	}


	void Awake() 
	{
		//init
		m_background = gameObject.GetComponent<Image>();
		rectTransform = gameObject.GetComponent<RectTransform>();
		m_portraite = transform.GetChild(0).GetComponent<Image>();

		m_background.color = Netural;
	}

	// this should replaced with listening for some events
	void Update()
	{
		// pop up the summary if the unit is selected
		if(m_gridModel.SelectedUnit == m_unit.model)
		{
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x,rectTransform.rect.height / 2,0);
		}
		else
		{
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x,0,0);
		}

		// highlight the summary if it is active/highlighted
		if(m_gridModel.HighlightedUnit == m_unit.model)
		{
			m_background.color = Highlighted;
		}
		else if(m_gridModel.ActiveUnit == m_unit.model)
		{
			m_background.color = Active;
		}
		else
		{
			m_background.color = Netural;
		}
	}
}
