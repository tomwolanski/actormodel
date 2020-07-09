using System;
using ActorSystem.Core;
using ActorSystem.PiCalculator.Messages;

namespace ActorSystem.PiCalculator.Actors
{
	public class PointGeneratorActor : ActorBase
	{
		private readonly Random _rnd;

		public PointGeneratorActor(ActorContext ctx) : base(ctx)
		{
			_rnd = new Random(Environment.TickCount * Self.Id.GetHashCode());
		}

		protected override void OnStarted()
		{
			Self.Tell(new GenerateNextPointMsg(), Self);
		}

		protected override void OnMessage(object message, ActorRef sender)
		{
			var x = _rnd.NextDouble();
			var y = _rnd.NextDouble();
			var r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

			var msg = new PointMsg(x, y, r <= 1);
			Parent.Tell(msg, Self);
			Self.Tell(new GenerateNextPointMsg(), Self);
		}
	}
}
