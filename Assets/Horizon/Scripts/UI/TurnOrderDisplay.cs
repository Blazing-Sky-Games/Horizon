using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class TurnOrderDisplay : MonoBehaviour 
{
	public List<HorizonUnitGameView> turnOrder = new List<HorizonUnitGameView>();

	private List<UnitSummary> summarys = new List<UnitSummary>();

	void Start()
	{
		UpdateTurnOrder();
	}

	// remove and reposision unit summaries in the turn order
	public void UpdateTurnOrder()
	{
		//get rid of all the old ones
		foreach(UnitSummary summary in summarys)
		{
			Destroy(summary.gameObject);
		}
		summarys.Clear();

		//make new summaries
		foreach(HorizonUnitGameView unit in turnOrder)
		{
			UnitSummary summary = CombatUI.NewUnitSummary();
			summary.SetUnit(unit);
			summary.rectTransform.SetParent(transform,false);
			summarys.Add(summary);
		}

		// do some layout stuff
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
}
