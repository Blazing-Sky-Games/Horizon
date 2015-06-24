using UnityEngine;
using UnityEditor;

using Gamelogic.Grids;

using System.Collections;
using System.Collections.Generic;

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

	private Stack<Color> colorStack = new Stack<Color>();

	public void pushHighlightColor(Color color)
	{
		colorStack.Push(color);
		HighlightColor = color;
	}

	public void popHighlightColor()
	{
		if(colorStack.Count > 0)
			colorStack.Pop();

		if(colorStack.Count > 0)
			HighlightColor = colorStack.Peek();
		else
			HighlightColor = new Color(0,0,0,0);
	}

	void Start()
	{
		m_model = gameObject.GetComponent<HorizonCellModel>();
	}

	void Update()
	{
		Color = HighlightColor;
	}
}
