using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Grids;

// all of the states a cell can be in
// will probably grow as the project gets more complicated
public enum CellState
{
	Passable, // units can walk through this cell
	NonPassable // units can not walk through this cell
};

// data describing a single cell
public class HorizonCellModel : MonoBehaviour 
{
	// serilizes field (it think) is needed for a field to be accses from editor scripts. maybe ... check this later
	[SerializeField]
	// what state is the cell in
	public CellState state;

	//these should probably be changed into properties instead of regualer fields
	// null if there is no unit in this cell, otherwise contains the unit model of the unit in this cell
	public HorizonUnitModel OccupyingUnit;
	// the coordinates of this cell
	public RectPoint PositionPoint;
}
