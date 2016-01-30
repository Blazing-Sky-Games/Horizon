using System;

public class Statistic
{
	public Statistic(int value)
	{
		m_value = value;;
	}

	public int Value
	{
		get
		{
			return m_value + m_modification;
		}
	}

	public void ApplyDrop(int modification)
	{
		m_modification -= modification;
	}
	public void RemoveDrop(int modification)
	{
		m_modification += modification;
	}
	public void ApplyBonus(int modification)
	{
		m_modification += modification;
	}
	public void RemoveBonus(int modification)
	{
		m_modification -= modification;
	}

	private int m_modification = 0;
	private int m_value;
}



