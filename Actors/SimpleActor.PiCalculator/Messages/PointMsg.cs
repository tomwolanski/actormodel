namespace SimpleActor.PiCalculator.Messages
{
	public sealed class PointMsg
	{
		public double X { get; }
		public double Y { get; }
		public bool InCircle { get; }

		public PointMsg(double x, double y, bool inCircle)
		{
			X = x;
			Y = y;
			InCircle = inCircle;
		}
	}
}
