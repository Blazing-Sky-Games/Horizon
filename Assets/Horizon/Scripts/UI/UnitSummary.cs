using UnityEngine;
using UnityEngine.UI;

using System.Collections;

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
		m_portraite.sprite = m_unit.Portrait;
	}

	public void SetGridModel(HorizonGridModel model)
	{
		m_gridModel = model;
	}


	void Awake() 
	{
		m_background = gameObject.GetComponent<Image>();
		rectTransform = gameObject.GetComponent<RectTransform>();
		m_portraite = transform.GetChild(0).GetComponent<Image>();

		m_background.color = Netural;
	}

	void Update()
	{
		if(m_gridModel.SelectedUnit == m_unit.model)
		{
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x,rectTransform.rect.height / 2,0);
		}
		else
		{
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x,0,0);
		}

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
