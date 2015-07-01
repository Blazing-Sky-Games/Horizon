using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Grids;

// mostly related to cell highlighting
// sprite cell is a class from the gamelogic plugin
// i think this could be reworked to be more light weight
public class HorizonCellGameView : SpriteCell 
{
	// public property exposing the model assaciated with this view
	public HorizonCellModel model
	{
		get
		{
			// find the model for a view by looking on the game object the view is attached to
			if(m_model == null) m_model = gameObject.GetComponent<HorizonCellModel>();

			return m_model;
		}
	}

	// the model backing feild
	private HorizonCellModel m_model;

	// what color is this cell in the game
	public Color HighlightColor = new Color(0,0,0,0);

	// used to keep trac of all the colors that have been "layered" on this cell
	// after writing the whole application using this stack based approche, i think it sucks, and it could be simplified
	// like just having a bunch of bools in the model corrisponding to differant highliting states. 
	// then the view would look at those and figure out its color.
	// we dont really need arbiratry highlight layering: we no all the differant highlighting we will be doing
	private Stack<Color> colorStack = new Stack<Color>();

	// set the color to a new color and save the old one
	public void pushHighlightColor(Color color)
	{
		colorStack.Push(color);
		HighlightColor = color;
	}

	// rest the color to the color it was befor
	public void popHighlightColor()
	{
		if(colorStack.Count > 0)
			colorStack.Pop();

		if(colorStack.Count > 0)
			HighlightColor = colorStack.Peek();
		else
			HighlightColor = new Color(0,0,0,0);
	}

	// called when a scene loads
	void Start()
	{
		// set the model ... wait ... why do i do the weird property thing then
		// hmm ... i might be doing this because an editor scrip touches the model throught a cell view
		// and start is not called in the editor
		m_model = gameObject.GetComponent<HorizonCellModel>();
	}

	// called every frame
	void Update()
	{
		// update the color
		// Color is a property enherited from spritecell
		Color = HighlightColor;

		//todo: make the sprite the right size even if the cell size changes
		//todo: use gl drawing function to draw the cells, not sprites
	}
}
