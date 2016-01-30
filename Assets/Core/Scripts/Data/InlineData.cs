using System;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class InlineData : Attribute
{
	public readonly bool SelectType;

	public InlineData(bool selectType = false)
	{
		SelectType = selectType;
	}
}
