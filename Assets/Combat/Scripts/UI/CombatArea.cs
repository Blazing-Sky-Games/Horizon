using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CombatArea : MonoBehaviour {

	public UnitView UnitViewPrefab;
	public Vector2 UnitStride;
	public Vector2 AIUnitOffset;
	public Vector2 PlayerUnitOffset;

	public void Init(TurnOrder TurnOrder ,MessageChannel<Unit> SelectUnitMessage)
	{
		m_turnOrder = TurnOrder;
		m_selectUnitMessage = SelectUnitMessage;

		UpdateCombatArea ();

		StartCoroutine (CombatAreaMain());
	}

	void UpdateCombatArea ()
	{
		PlayerUnits.Clear ();
		AIUnits.Clear ();

		foreach (Unit unit in m_turnOrder)
		{
			if(unit.Faction == Faction.Player)
			{
				PlayerUnits.Add(unit);
			}
			else if(unit.Faction == Faction.AI)
			{
				AIUnits.Add(unit);
			}
		}

		foreach (UnitView view in PlayerUnitViews)
		{
			//hmm....we have to wait for all pending messages to be done
			Destroy(view.gameObject);
		}

		PlayerUnitViews.Clear ();

		foreach(Unit unit in PlayerUnits)
		{
			PlayerUnitViews.Add (InstantiateUnitView (unit));
		}

		foreach (UnitView view in AIUnitViews)
		{
			Destroy(view.gameObject);
		}
		
		AIUnitViews.Clear ();
		
		foreach(Unit unit in AIUnits)
		{
			AIUnitViews.Add (InstantiateUnitView (unit));
		}

		for(int i = 0; i < PlayerUnitViews.Count; i++)
		{
			PlayerUnitViews[i].transform.localPosition = new Vector3(PlayerUnitOffset.x + i * UnitStride.x,PlayerUnitOffset.y + i * UnitStride.y,0);
		}
		for(int i = 0; i < AIUnitViews.Count; i++)
		{
			AIUnitViews[i].transform.localPosition = new Vector3(AIUnitOffset.x + i * UnitStride.x, AIUnitOffset.y + i * UnitStride.y,0);
		}
	}

	private IEnumerator CombatAreaMain()
	{
		while (true)
		{
			yield return m_turnOrder.UnitKilledMessage.WaitForMessage();
			yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
			//UpdateCombatArea(); MEGA BUG
		}
	}

	private UnitView InstantiateUnitView(Unit unit)
	{
		UnitView newView = Instantiate<UnitView> (UnitViewPrefab);
		newView.Init(unit,m_selectUnitMessage,m_turnOrder);
		newView.transform.SetParent (transform, false);
		newView.transform.localScale = new Vector3 (1, 1, 1);

		return newView;
	}

	private List<Unit> PlayerUnits = new List<Unit>();
	private List<UnitView> PlayerUnitViews = new List<UnitView> ();
	private List<Unit> AIUnits = new List<Unit>();
	private List<UnitView> AIUnitViews = new List<UnitView> ();
	private MessageChannel<Unit> m_selectUnitMessage;
	private TurnOrder m_turnOrder;
}
