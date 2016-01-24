using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CombatAreaView : View<CombatMainLogic,CombatLogicData,EmptyData>
{
	//supplyed in editor
	public UnitView UnitViewPrefab;
	public Vector2 UnitStride;
	public Vector2 AIUnitOffset;
	public Vector2 PlayerUnitOffset;

	protected override void SetUp ()
	{
		base.SetUp();
		turnOrder = ServiceUtility.GetServiceReference<TurnOrder>();
	}

	protected override IEnumerator MainRoutine ()
	{
		//init
		UpdateCombatArea();

		while(true)
		{
			//wait for a message we care about
			//yield return m_turnOrder.UnitKilledMessage.WaitForMessage();
			//yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
			//UpdateCombatArea(); TODO MEGA BUG
			yield return new WaitForNextFrame();
		}
	}

	// remove dead units, and reposition units
	// TODO fix bug related to this
	void UpdateCombatArea()
	{
		// clear unit lists
		m_playerUnits.Clear();
		m_aIUnits.Clear();

		// add units from turn order to lists
		foreach(UnitLogic unit in turnOrder.Dereference())
		{
			if(unit.Faction == Faction.Player)
			{
				m_playerUnits.Add(unit);
			}
			else if(unit.Faction == Faction.AI)
			{
				m_aIUnits.Add(unit);
			}
		}

		// remove the old unit views
		foreach(UnitView view in m_playerUnitViews)
		{
			//hmm....we have to wait for all pending messages to be done
			Destroy(view.gameObject);
		}
		m_playerUnitViews.Clear();

		foreach(UnitView view in m_aIUnitViews)
		{
			Destroy(view.gameObject);
		}
		m_aIUnitViews.Clear();

		//create new unit views
		foreach(UnitLogic unit in m_playerUnits)
		{
			m_playerUnitViews.Add(InstantiateUnitView(unit));
		}
		
		foreach(UnitLogic unit in m_aIUnits)
		{
			m_aIUnitViews.Add(InstantiateUnitView(unit));
		}

		// position unit views
		for(int i = 0; i < m_playerUnitViews.Count; i++)
		{
			m_playerUnitViews[i].transform.localPosition = new Vector3(PlayerUnitOffset.x + i * UnitStride.x, PlayerUnitOffset.y + i * UnitStride.y, 0);
		}
		for(int i = 0; i < m_aIUnitViews.Count; i++)
		{
			m_aIUnitViews[i].transform.localPosition = new Vector3(AIUnitOffset.x + i * UnitStride.x, AIUnitOffset.y + i * UnitStride.y, 0);
		}
	}

	//creat a new unit view
	private UnitView InstantiateUnitView(UnitLogic unit)
	{
		UnitView newView = Instantiate<UnitView>(UnitViewPrefab);
		newView.transform.SetParent(transform, false);
		newView.transform.localScale = new Vector3(1, 1, 1);

		return newView;
	}

	private readonly  List<UnitLogic> m_playerUnits = new List<UnitLogic>();
	private readonly List<UnitView> m_playerUnitViews = new List<UnitView>();
	private readonly List<UnitLogic> m_aIUnits = new List<UnitLogic>();
	private readonly List<UnitView> m_aIUnitViews = new List<UnitView>();

	WeakReference<TurnOrder> turnOrder;
}
