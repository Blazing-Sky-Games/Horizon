using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class LightsOutCell : SpriteCell 
{
	public bool IsOn
	{
		get { return m_isOn; }
		set
		{
			m_isOn = value;
			Color = m_isOn ? Globals.instance.onColor : Globals.instance.offColor;
		}
	}

	public void OnClicked()
	{
		//Do Nothing
	}

	private bool m_isOn;
}
