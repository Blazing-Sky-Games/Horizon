using System;

public class EmptyLogic : ViewLogic<EmptyData>
{
	public static EmptyLogic Empty;

	static EmptyLogic()
	{
		Empty = new EmptyLogic(Data.Empty);
	}

	public EmptyLogic(EmptyData data): base(data){}
}



