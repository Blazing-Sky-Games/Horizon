using UnityEngine;

using System.Collections;

using Gamelogic.Grids;

public class HorizonGridModel : GridBehaviour<RectPoint> 
{
	//unity messages

	//

	private IGrid<HorizonCellView, RectPoint> cellViewGrid;

	public IGrid<HorizonCellView, RectPoint> CellViewGrid
	{
		get
		{
			if(cellViewGrid == null)
			{
				cellViewGrid = Grid.CastValues<HorizonCellView, RectPoint>();
			}

			return cellViewGrid;
		}
	}

	public override void InitGrid ()
	{
		cellViewGrid = Grid.CastValues<HorizonCellView, RectPoint>();
	}

	//grid specific messages
}
