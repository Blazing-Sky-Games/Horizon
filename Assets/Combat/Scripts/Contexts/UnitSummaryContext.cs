using UnityEngine;
using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public class UnitSummaryContext : Context
{
	public UnitSummaryContext()
	{
		SetUnit(null);
	}

	public UnitSummaryContext(UnitLogic unit)
	{
		SetUnit(unit);
	}

	private UnitLogic m_unit;

	public void SetUnit(UnitLogic unit)
	{
		if(m_unit != null)
		{
			detachUnit();
		}

		m_unitPortrait.Value = null;
		m_hpPercent.Value = 1;
		m_displayString.Value = "?/?";

		m_unit = unit;

		if(m_unit != null)
		{
			attachUnit();
		}
	}

	private void detachUnit()
	{
		m_unit.Health.MaxChanged.RemoveAction(UnitHealthChanged);
		m_unit.Health.Hurt.RemoveAction(UnitHealthChanged);
	}

	private void attachUnit()
	{
		UnitHealthChanged();
		m_unit.Health.MaxChanged.AddAction(UnitHealthChanged);
		m_unit.Health.Hurt.AddAction(UnitHealthChanged);

		//get the unit potraite
		//m_unitPortrait.Value = UnitPortraiteService...
	}

	void UnitHealthChanged()
	{
		m_hpPercent.Value = (float)m_unit.Health.Current / (float)m_unit.Health.Max;
		m_displayString.Value = m_unit.Health.Current + " / " + m_unit.Health.Max;
	}

	private readonly Property<Sprite> m_unitPortrait = new Property<Sprite>();
	private readonly Property<float> m_hpPercent = new Property<float>();
	private readonly Property<string> m_displayString = new Property<string>();

	public Sprite UnitPortrait
	{
		get
		{
			return m_unitPortrait.Value;
		}
	}
	public float HpPercent
	{
		get
		{
			return m_hpPercent.Value;
		}
	}
	public string DisplayString
	{
		get
		{
			return m_displayString.Value;
		}
	}
}

