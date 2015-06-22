using UnityEngine;

using System.Collections;

using Gamelogic.Grids;

public enum UnitDirection
{
	forward,
	backward,
	left,
	right
}

public class HorizonUnitModel : MonoBehaviour 
{
	public RectPoint PositionPoint;

	[SerializeField]
	public int X;

	[SerializeField]
	public int Y;

	[SerializeField]
	public UnitDirection Direction;

	[SerializeField]
	public HorizonGridView GridView;

	[SerializeField]
	public int speed;

	[SerializeField]
	public int Hp;

	[SerializeField]
	public int maxHp;
}
