using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CombatArea : MonoBehaviour
{
	//supplyed in editor
	public UnitView UnitViewPrefab;
	public Vector2 UnitStride;
	public Vector2 AIUnitOffset;
	public Vector2 PlayerUnitOffset;

	// call to supply dependancies
	public void Init(TurnOrder turnOrder, Message<Unit> selectUnitMessage)
	{
		//set backing fields
		m_turnOrder = turnOrder;
		m_selectUnitMessage = selectUnitMessage;

		//init
		UpdateCombatArea();

		//start main
		CoroutineManager.Main.StartCoroutine(WaitCombatAreaMain());
	}

	// remove dead units, and reposition units
	// TODO fix bug related to this
	void UpdateCombatArea()
	{
		// clear unit lists
		m_playerUnits.Clear();
		m_aIUnits.Clear();

		// add units from turn order to lists
		foreach(Unit unit in m_turnOrder)
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
		foreach(Unit unit in m_playerUnits)
		{
			m_playerUnitViews.Add(InstantiateUnitView(unit));
		}
		
		foreach(Unit unit in m_aIUnits)
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

	private IEnumerator WaitCombatAreaMain()
	{
		while(true)
		{
			//wait for a message we care about
			//yield return m_turnOrder.UnitKilledMessage.WaitForMessage();
			//yield return m_turnOrder.UnitKilledMessage.WaitTillMessageProcessed();
			//UpdateCombatArea(); TODO MEGA BUG
			yield return 0;
		}
	}

	//creat a new unit view
	private UnitView InstantiateUnitView(Unit unit)
	{
		UnitView newView = Instantiate<UnitView>(UnitViewPrefab);
		newView.Init(unit, m_selectUnitMessage, m_turnOrder);
		newView.transform.SetParent(transform, false);
		newView.transform.localScale = new Vector3(1, 1, 1);

		return newView;
	}

	private readonly  List<Unit> m_playerUnits = new List<Unit>();
	private readonly List<UnitView> m_playerUnitViews = new List<UnitView>();
	private readonly List<Unit> m_aIUnits = new List<Unit>();
	private readonly List<UnitView> m_aIUnitViews = new List<UnitView>();
	private Message<Unit> m_selectUnitMessage;
	private TurnOrder m_turnOrder;
}
