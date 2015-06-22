using UnityEngine;
using System.Collections;

public enum CellState
{
	Passable,
	NonPassable
};

public class HorizonCellModel : MonoBehaviour 
{
	public CellState state;
}
