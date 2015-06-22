using UnityEngine;

using System.Collections;
using System.Collections.Generic;

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
}
