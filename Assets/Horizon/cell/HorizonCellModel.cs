using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Gamelogic.Grids;

public enum CellState
{
	Passable,
	NonPassable
};

public class HorizonCellModel : MonoBehaviour 
{
	[SerializeField]
	public CellState state;
	//public List<HorizonUnitModel> Units;

	public RectPoint PositionPoint;
}
