using System;

public abstract class DataDrivenLogic<DataType> 
	where DataType : Data
{
	protected readonly DataType Data;

	public DataDrivenLogic(DataType Data)
	{
		this.Data = Data;
	}
}


