namespace EntityDiffSample
{
	public class BrokenRule
	{
		public string Message { get; private set; }

		public BrokenRule(string message)
		{
			Message = message;
		}
	}
}