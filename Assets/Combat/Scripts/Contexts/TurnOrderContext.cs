using UnityEngine;
using System.Collections;
using Slash.Unity.DataBind.Core.Data;

public class TurnOrderContext : Context 
{
	private readonly Collection<UnitSummaryContext> m_units = new Collection<UnitSummaryContext>();

	public Collection<UnitSummaryContext> Units
	{
		get
		{
			return m_units;
		}
	}

	private readonly Property<int> m_highlightedIndex = new Property<int>();

	public int HighlightedIndex
	{
		get
		{
			return m_highlightedIndex.Value;
		}
		set
		{
			m_highlightedIndex.Value = value;
		}
	}

	public Message msgProp
	{
		get
		{
			return m_mp;
		}
	}

	private Message m_mp = new Message();
}
