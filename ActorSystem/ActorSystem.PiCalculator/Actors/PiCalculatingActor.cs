using System;
using System.Linq;
using ActorSystem.Core;
using ActorSystem.Core.Supervision;
using ActorSystem.PiCalculator.Messages;

namespace ActorSystem.PiCalculator.Actors
{
	public class PiCalculatingActor : ActorBase
	{
		private ulong totalPoints = 0;
		private ulong inCirclePoints = 0;

		private readonly ActorRef[] _generators;

		public PiCalculatingActor(ActorContext ctx, int workerCount) : base(ctx)
		{
			_generators = Enumerable.Range(0, workerCount)
				.Select(n => Create($"gen{n}", ctx => new PointGeneratorActor(ctx)))
				.ToArray();
		}

		protected override void OnMessage(object message, ActorRef sender)
		{
			switch (message)
			{
				case PointMsg msg:
					{
						totalPoints++;
						if (msg.InCircle)
						{
							inCirclePoints++;
						}
						break;
					}
				case GetCurrentPiMsg _:
					{
						var pi = 4.0 * (double)inCirclePoints / (double)totalPoints;
						var err = Math.Abs(Math.PI - pi);
						sender.Tell(new CurrentPiMsg(pi, err), Self);
						break;
					}
			}
		}

		// protected override ExceptionHandelingStrategy OnChildException(object message, Exception ex) => ExceptionHandelingStrategy.RestartChild;
	}
}
