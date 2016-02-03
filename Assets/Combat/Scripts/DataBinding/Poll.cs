public class Poll
{
	private int numVotes = 0;

	public void AddVote ()
	{
		numVotes++;
	}

	public void RemoveVote ()
	{
		numVotes--;
	}

	public bool AnyVotes
	{
		get
		{
			return numVotes > 0;
		}
	}

	public int VoteCount
	{
		get
		{
			return numVotes;
		}
	}
}



