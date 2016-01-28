public class Poll
{
	private int numFalse = 0;
	private int numTrue = 0;

	public void AddVote (bool vote)
	{
		if(vote)
			numTrue++;
		else
			numFalse++;
	}

	public void RemoveVote (bool vote)
	{
		if(vote)
			numTrue--;
		else
			numFalse--;
	}

	public bool AnyFalse
	{
		get
		{
			return numFalse > 0;
		}
	}

	public bool AnyTrue
	{
		get
		{
			return numTrue > 0;
		}
	}

	public bool MajorityVote
	{
		get
		{
			return numTrue > numFalse;
		}
	}

	public bool MinorityVote
	{
		get
		{
			return !MajorityVote;
		}
	}

	public int VoteCount
	{
		get
		{
			return numTrue + numFalse;
		}
	}

	public int TrueCount
	{
		get
		{
			return numTrue;
		}
	}

	public int FalseCount
	{
		get
		{
			return numFalse;
		}
	}
}



