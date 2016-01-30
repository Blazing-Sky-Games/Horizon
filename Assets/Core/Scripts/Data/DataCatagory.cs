using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DataCatagory : Attribute
{
	//tag data type with this to control how it displays in create data menu
	// for example "combat" will appear in a combat drop down
	// "combat/logic" will appear in a drop down inside the dropdown combat

	public readonly string Catagory;

	public DataCatagory(string catagory)
	{
		Catagory = catagory;
	}
}
