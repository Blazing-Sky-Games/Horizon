using UnityEngine;
using UnityEditor;

using Gamelogic.Grids;

using System.Collections;

public class HorizonCellView : SpriteCell 
{
	public HorizonCellModel model = null;

	public Color HighlightColor = new Color(0,0,0,0);

	void Start()
	{
		model = gameObject.GetComponent<HorizonCellModel>();
	}

	void Update()
	{
		Color = HighlightColor;
	}
}
