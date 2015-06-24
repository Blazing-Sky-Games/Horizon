using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
	private RectPoint positionPoint;

	public RectPoint PositionPoint
	{
		get
		{
			return positionPoint;
		}
		set
		{
			if(value != positionPoint) pointsInMovmentRange = null;
			positionPoint = value;
		}
	}

	private List<RectPoint> pointsInMovmentRange;

	public IEnumerable<RectPoint> PointsInMovmentRange
	{
		get
		{
			if(pointsInMovmentRange == null) pointsInMovmentRange = GetPointsInMovementRange();

			return pointsInMovmentRange;
		}
	}

	public List<RectPoint> GetPointsInMovementRange()
	{
		return GridView.model.GetPointsInRangeCost(
			PositionPoint,
			speed,
			(x) => x.model.state == CellState.Passable,
			(x,y) => {return 1;}
		).Select(x => x.Key).ToList();
	}

	void Awake()
	{
		PositionPoint = new RectPoint(X,Y);
	}

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
