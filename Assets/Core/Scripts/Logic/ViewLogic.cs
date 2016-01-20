using System;

public abstract class ViewLogic<DataType> 
	where DataType : Data
{
	//only accsesable from contructor
	protected readonly DataType data_copy;

	public ViewLogic(DataType Data)
	{
		this.data_copy = (DataType)Data.DeepCopy();
	}

	// unsubscribe from messages, etc
	public virtual void Destroy(){}
}


