using UnityEngine;
using UnityEditor;

using Gamelogic.Grids;

using System.Collections;

public class HorizonCellView : SpriteCell 
{
	public HorizonCellModel model
	{
		get
		{
			if(m_model == null) m_model = gameObject.GetComponent<HorizonCellModel>();

			return m_model;
		}
	}

	private HorizonCellModel m_model;

	public Color HighlightColor = new Color(0,0,0,0);

	void Start()
	{
		m_model = gameObject.GetComponent<HorizonCellModel>();
	}

	void Update()
	{
		Color = HighlightColor;
	}
}
