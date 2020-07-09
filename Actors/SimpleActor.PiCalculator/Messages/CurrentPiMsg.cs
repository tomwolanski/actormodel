namespace SimpleActor.PiCalculator.Messages
{
	public sealed class CurrentPiMsg
	{
		public double Value { get; }
		public double Error { get; }

		public CurrentPiMsg(double value, double error)
		{
			Value = value;
			Error = error;
		}
	}
}
