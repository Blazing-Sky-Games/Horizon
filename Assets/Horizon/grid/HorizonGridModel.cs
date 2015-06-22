using UnityEngine;

using System.Collections;

using Gamelogic.Grids;

public class HorizonGridModel : GridBehaviour<RectPoint> 
{
	//unity messages

	//

	public IGrid<HorizonCellView, RectPoint> CellViewGrid;

	public override void InitGrid ()
	{
		CellViewGrid = Grid.CastValues<HorizonCellView, RectPoint>();
	}

	//grid specific messages
}
