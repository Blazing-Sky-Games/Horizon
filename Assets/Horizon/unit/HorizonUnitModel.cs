using UnityEngine;

using System;
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

public enum UnitType
{
	Character,
	Enemy
}

public class Referance<ValueType>
{
	public ValueType value;
}

public static class TransformRoutines
{
	public static IEnumerator MoveTransformToPointInTime(this Transform tran, Vector3 Destination, float time)
	{
		Vector3 start = tran.position;
		float elapsed = 0;
		while(elapsed < time)
		{
			tran.position = Vector3.Lerp(start,Destination,elapsed/time);
			elapsed += Time.deltaTime;
			yield return null;
		}
		tran.position = Destination;
	}
}

public class HorizonUnitModel : MonoBehaviour 
{
	private RectPoint positionPoint;

	public HorizonCellModel OccupyingCell;

	public static event Action<HorizonUnitModel> OnUnitHPEqualsZero;

	public event Action OnTurnFinished;

	public HorizonUnitView view;

	public int AttackPower = 2;

	public bool hasMoved = false;
	public bool hasAttacked = false;

	public void PassTurn ()
	{
		hasMoved = false;
		hasAttacked = false;
		pointsInMovmentRange = null;
		if(OnTurnFinished != null) OnTurnFinished();
	}

	public void Attack(HorizonUnitModel unit)
	{
		if(hasAttacked) return;

		// hmm ... code here to wait for an animation to finish

		unit.Hp -= AttackPower;

		hasAttacked = true;
		if(hasMoved && hasAttacked) PassTurn();
	}

	public RectPoint PositionPoint
	{
		get
		{
			return positionPoint;
		}
		set
		{
			pointsInMovmentRange = null;

			if(OccupyingCell != null)
			{
				OccupyingCell.OccupyingUnit = null;
			}

			OccupyingCell = GridView.model.CellViewGrid[value].model;
			OccupyingCell.OccupyingUnit = this;

			positionPoint = value;

			X = positionPoint.X;
			Y = positionPoint.Y;
		}
	}

	private IEnumerable<RectPoint> pointsInMovmentRange;

	public IEnumerable<RectPoint> PointsInMovmentRange
	{
		get
		{
			if(pointsInMovmentRange == null) pointsInMovmentRange = GetPointsInMovementRange();

			return pointsInMovmentRange;
		}
	}

	public IEnumerable<RectPoint> GetPointsInMovementRange()
	{
		if (hasMoved == true) return new List<RectPoint>();

		return GridView.model.GetPointsInRangeCost(
			PositionPoint,
			speed,
			CanPassPoint,
			(startPoint,endPoint) => {return 1;},
			(point,moveCost) => 
			{
				HorizonCellModel cell = GridView.model.CellViewGrid[point].model;
				if(cell.state == CellState.Passable)
				{
					return cell.OccupyingUnit == null;
				}
				else
				{
					return false;
				}
			}
		);
	}

	private bool CanPassPoint(HorizonCellView cell)
	{
		return 	 cell.model.state == CellState.Passable && 
				(cell.model.OccupyingUnit == null || cell.model.OccupyingUnit.unitType == unitType);
	}

	public IEnumerable<RectPoint> ShortestPathToPoint(RectPoint point)
	{
		return GridView.model.ShortestPathBetweenPoints(PositionPoint, point, CanPassPoint);
	}

	private IEnumerator TryMoveToCell(RectPoint point, Referance<bool> result)
	{
		bool cellWasEmpty = GridView.model.CellViewGrid[point].model.OccupyingUnit == null;

		if(GridView.Grid.GetAllNeighbors(PositionPoint).Contains(point))
		{
			RectPoint direction = point - PositionPoint;
			if(direction == RectPoint.North)
			{
				SetUnitDirection(UnitDirection.forward);
			}
			else if(direction == RectPoint.South)
			{
				SetUnitDirection(UnitDirection.backward);
			}
			else if(direction == RectPoint.East)
			{
				SetUnitDirection(UnitDirection.right);
			}
			else if(direction == RectPoint.West)
			{
				SetUnitDirection(UnitDirection.left);
			}

			yield return StartCoroutine(transform.MoveTransformToPointInTime(GridView.Grid[point].Center,0.5f));

			if(cellWasEmpty)
			{
				// use the property so we reset cell ocupation
				PositionPoint = point;
			}
			else
			{
				// we are passing through a freindly unit, dont edit cell ocupation, so dont use the property
				positionPoint = point;
			}

			result.value = true;
		}
		else
		{
			result.value = false;
		}
	}

	// event on trverspathstart ... use for animation
	public event Action OnTraversePathEnd;
	public IEnumerator TraversePath(IEnumerable<RectPoint> path)
	{
		//trverspathstart();
		if(path != null)
		{
			foreach(RectPoint point in path)
			{
				Referance<bool> result = new Referance<bool>();
				yield return StartCoroutine(TryMoveToCell(point,result));
				
				if(result.value == false) break;
			}
		}

		if(OnTraversePathEnd != null) 
		{
			hasMoved = true;
			pointsInMovmentRange = null;
			OnTraversePathEnd();
			if(hasMoved && hasAttacked) PassTurn();
		}
	}

	public IEnumerator TraverseShortestPathToPoint(RectPoint point)
	{
		yield return StartCoroutine(TraversePath(ShortestPathToPoint(point)));
	}

	void Awake()
	{
		PositionPoint = new RectPoint(X,Y);
		Hp = maxHp;
		lastHp = maxHp;
	}

	[SerializeField]
	public int X;

	[SerializeField]
	public int Y;

	[SerializeField]
	public UnitDirection Direction;

	[SerializeField]
	public UnitType unitType;

	public void SetUnitDirection(UnitDirection direction)
	{
		if(direction != Direction)
		{
			switch(direction)
			{
			case UnitDirection.forward:
				transform.rotation = Quaternion.identity;
				break;
			case UnitDirection.backward:
				transform.rotation = Quaternion.Euler(0,180,0);
				break;
			case UnitDirection.left:
				transform.rotation = Quaternion.Euler(0,-90,0);
				break;
			case UnitDirection.right:
				transform.rotation = Quaternion.Euler(0,90,0);
				break;
			}
		}

		Direction = direction;
	}

	[SerializeField]
	public HorizonGridView GridView;

	[SerializeField]
	public int speed;

	[SerializeField]
	public int Hp;

	private int lastHp;
	void Update()
	{
		Hp = (int)Mathf.Clamp(Hp,0,maxHp);

		if(lastHp != Hp && Hp == 0)
		{
			if(OnUnitHPEqualsZero != null)
				OnUnitHPEqualsZero(this);
		}

		lastHp = Hp;
	}

	[SerializeField]
	public int maxHp;
}
