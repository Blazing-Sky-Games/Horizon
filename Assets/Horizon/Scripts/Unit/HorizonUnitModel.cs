using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gamelogic.Grids;

//which way is the unit facing
public enum UnitDirection
{
	forward,
	backward,
	left,
	right
}

// is the unit a good guy or a bad guy
public enum UnitType
{
	Character,
	Enemy
}

// umm ... this is a weird thing i did to pass a bool as an out parameter from a corutine ...
public class Referance<ValueType>
{
	public ValueType value;
}

// extention method on transform
public static class TransformRoutines
{
	// lineraly move a ameobject to apoint in an amount of time
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
	private RectPoint positionPoint; // where is this unit

	public HorizonCellModel OccupyingCell; // what cell is this unit in

	public static event Action<HorizonUnitModel> OnUnitHPEqualsZero; // fired when any unit dies, passing the unit that died

	public event Action OnTurnFinished; // fired when this unit is done with its turn

	public HorizonUnitGameView view; //UUUGGGHHHHH this is so bad, why does the model have a referance to the view. i dont care if i use this in a place, that is just a sign that the larger architecture is crap

	public int AttackPower = 2; // how much damage is dont when this unit attacks

	public bool hasMoved = false; // have we moved this turn
	public bool hasAttacked = false; // have we attacked this turn

	public void PassTurn ()
	{
		// reset bools
		hasMoved = false;
		hasAttacked = false;

		//clear movment range ... i forget why
		pointsInMovmentRange = null;

		//fire on turn finished
		if(OnTurnFinished != null) OnTurnFinished();
	}

	public void Attack(HorizonUnitModel unit)
	{
		if(hasAttacked) return;

		// turn to face the direction we are attacking
		RectPoint direction = unit.PositionPoint - positionPoint;
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

		// hmm ... code here to wait for an animation to finish
		// turn attack into a corutine

		unit.Hp -= AttackPower;

		hasAttacked = true;
		if(hasMoved && hasAttacked) PassTurn(); // if we have attacked and moved, pass turn
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

			// update cell ocupation
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
			// if the value is null, recompute
			// this lets us et the value to null to force a recompute
			if(pointsInMovmentRange == null) pointsInMovmentRange = GetPointsInMovementRange();

			return pointsInMovmentRange;
		}
	}

	// wrapper around the grid method to get points in a range
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
				// is a point a valid ending square

				HorizonCellModel cell = GridView.model.CellViewGrid[point].model;
				// you can only end on a cell if it is Passable
				if(cell.state == CellState.Passable)
				{
					// you can only end on a cell if it is not occupied
					return cell.OccupyingUnit == null;
				}
				else
				{
					return false;
				}
			}
		);
	}

	// return true if the point is passable and does not have an enemy unit in it
	private bool CanPassPoint(HorizonCellGameView cell)
	{
		return 	 cell.model.state == CellState.Passable && 
				(cell.model.OccupyingUnit == null || cell.model.OccupyingUnit.unitType == unitType);
	}

	// wrapper around grid shortestpath to point method
	public IEnumerable<RectPoint> ShortestPathToPoint(RectPoint point)
	{
		return GridView.model.ShortestPathBetweenPoints(PositionPoint, point, CanPassPoint);
	}

	// attempts to move to a cell
	// failes if the cell is not aneighbor
	// we could make this fail if we wanted to prematurly stop a path traversal
	private IEnumerator TryMoveToCell(RectPoint point, Referance<bool> result)
	{
		// did the cell have a unit in it
		bool cellWasEmpty = GridView.model.CellViewGrid[point].model.OccupyingUnit == null;

		if(GridView.Grid.GetAllNeighbors(PositionPoint).Contains(point))
		{
			// turn to face the direction of movment
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

			// move in space to the new point
			yield return StartCoroutine(transform.MoveTransformToPointInTime(GridView.Grid[point].Center,0.5f));

			//HACK: this is to fix a bug where unit movment screwed up cell ocupation
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
		// the cell was not a neighbor
		else
		{
			result.value = false;
		}
	}

	// event on trverspathstart ... add later to use for animation
	public event Action OnTraversePathEnd; // fired when this unit is done moving
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
			hasMoved = true; // we just moved
			pointsInMovmentRange = null; // invalideate the points in range
			OnTraversePathEnd(); // fire evnent
			if(hasMoved && hasAttacked) PassTurn(); // if we have moved and attacked, pass turn
		}
	}

	// utility function that wraps a common operation
	public IEnumerator TraverseShortestPathToPoint(RectPoint point)
	{
		yield return StartCoroutine(TraversePath(ShortestPathToPoint(point)));
	}

	void Awake()
	{
		PositionPoint = new RectPoint(X,Y); // oh yeah ... this weird ness is realted to the editor scipt
		Hp = maxHp;
		lastHp = maxHp;
	}

	[SerializeField]
	public int X; // for editor

	[SerializeField]
	public int Y; // for editor

	[SerializeField]
	public UnitDirection Direction; // which way is this unit facing

	[SerializeField]
	public UnitType unitType; // is this a character or an enemy

	// rotate the unit to face to correct direction
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
	public HorizonGridGameView GridView; // ughhhhh ... this is bad. it should at least be a grid model instead of a view

	[SerializeField]
	public int speed; // how many squares can we move

	[SerializeField]
	public int Hp; // how much health do we have

	private int lastHp;
	// called every frame
	void Update()
	{
		//HACK: we do this polling of the HP to know ehn to fire the death event
		// we should really just wrap hp in a property that handles the event firing
		// i didnt do that because then i would have to go add stuff to the editor script to show the propert, because unity doesnt display properties in the ditor by default
		Hp = (int)Mathf.Clamp(Hp,0,maxHp);

		if(lastHp != Hp && Hp == 0)
		{
			if(OnUnitHPEqualsZero != null)
				OnUnitHPEqualsZero(this);
		}

		lastHp = Hp;
	}

	[SerializeField]
	public int maxHp; // how much health do we start with
}
