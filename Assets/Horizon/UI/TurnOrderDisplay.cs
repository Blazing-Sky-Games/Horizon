using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class TurnOrderDisplay : MonoBehaviour 
{
	public List<HorizonUnitView> turnOrder = new List<HorizonUnitView>();

	private List<UnitSummary> summarys = new List<UnitSummary>();

	//private int m_activeUnitIndex;

	//public int ActiveUnitIndex
	//{
		//get
		//{
			//return m_activeUnitIndex;
		//}
	//}

	void Start()
	{
		UpdateTurnOrder();
	}

	public void UpdateTurnOrder()
	{
		foreach(UnitSummary summary in summarys)
		{
			Destroy(summary.gameObject);
		}

		summarys.Clear();

		foreach(HorizonUnitView unit in turnOrder)
		{
			UnitSummary summary = CombatUI.NewUnitSummary();
			summary.SetUnit(unit);
			summary.rectTransform.SetParent(transform,false);
			summarys.Add(summary);
		}

		float totalWidth = 0;
		foreach(UnitSummary summary in summarys)
		{
			totalWidth += summary.rectTransform.rect.width;
		}

		float AccumulatedWidth = 0;
		foreach(UnitSummary summary in summarys)
		{
			summary.rectTransform.localPosition = new Vector3(AccumulatedWidth - totalWidth/2 + summary.rectTransform.rect.width/2, summary.rectTransform.localPosition.y, 0);
			AccumulatedWidth += summary.rectTransform.rect.width;
		}
	}

	//public void IncrementActiveUnit()
	//{
		//m_activeUnitIndex++;
		//while(m_activeUnitIndex >= turnOrder.Count)
			//m_activeUnitIndex -= turnOrder.Count;

		//update Arrow .. hmm ... add arrow later
		//hmm we need some way to know which unit is the active one ...
	//}
}
