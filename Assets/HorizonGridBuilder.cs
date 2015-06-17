using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

[ExecuteInEditMode]
public class HorizonGridBuilder : RectTileGridBuilder
{
	void Awake()
	{

	}

	protected override void InitGrid ()
	{
		//called whenever the grid gets reset
		//NEED TO MAKE SURE CELL PROPS STAY 
		
		updateType = UpdateType.EditorAuto;
		plane = MapPlane.XZ;
		Alignment = MapAlignment.BottomLeft;
		cellSpacingFactor = new Vector2(1,1);
		
		base.Grid = (RectGrid<TileCell>)RectGrid<HorizonCell>.Rectangle(Dimensions.X, Dimensions.Y).CastValues<TileCell,RectPoint>();
		
		Grid.SetNeighborsMain();
	}

	new void Update()
	{
		ProcessInput();
	}

	private void ProcessInput()
	{
		if (Input.GetMouseButtonDown(0))
		{

		}
		
		if (Input.GetMouseButtonDown(1))
		{

		}
		
		if (Input.GetMouseButtonDown(2))
		{

		}

		//other input processing
	}
}
