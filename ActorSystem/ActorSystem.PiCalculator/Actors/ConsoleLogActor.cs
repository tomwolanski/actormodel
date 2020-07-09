using System;
using System.IO;
using ActorSystem.Core;
using ActorSystem.PiCalculator.Messages;

namespace ActorSystem.PiCalculator.Actors
{
	public class ConsoleLogActor : ActorBase
	{
		private readonly ActorRef _calc;
		private readonly TextWriter _writer;

		public ConsoleLogActor(ActorContext ctx, TextWriter writer, ActorRef calc) : base(ctx)
		{
			_calc = calc;
			_writer = writer;
		}

		protected override void OnStarted()
		{
			base.OnStarted();
			Self.ScheduleMessage(100, new RefreshLogMsg(), Self);
		}

		protected override void OnMessage(object message, ActorRef sender)
		{
			switch (message)
			{
				case CurrentPiMsg pi:
					{
						_writer.WriteLine($"pi: {pi.Value:N20}			err: {pi.Error:E10}");
						break;
					}
				case RefreshLogMsg _:
					{
						_calc.Tell(new GetCurrentPiMsg(), Self);
						Self.ScheduleMessage(100, new RefreshLogMsg(), Self);
						break;
					}
			}
		}
	}
}
