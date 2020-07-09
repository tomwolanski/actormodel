using SimpleActor.Core;
using SimpleActor.PiCalculator.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleActor.PiCalculator
{
    static class Program
    {
		public static async Task Main()
		{
			var actor = GetPiCalculatingActor();

			new Thread(() => GenerateRandomPoints(actor)).Start();
			new Thread(() => GenerateRandomPoints(actor)).Start();
			new Thread(() => GenerateRandomPoints(actor)).Start();
			new Thread(() => GenerateRandomPoints(actor)).Start();

			while (true)
			{
				await Task.Delay(100);
				var pi = await actor.AskAsync<CurrentPiMsg>(new GetCurrentPiMsg());

				Console.WriteLine($"pi: {pi.Value:N20}			err: {pi.Error:E10}");
			}
		}

		private static Actor GetPiCalculatingActor()
		{
			ulong totalPoints = 0;
			ulong inCirclePoints = 0;

			return new Actor((msg, sender) =>
			{
				switch (msg)
				{
					case PointMsg pointMsg:
						{
							totalPoints++;

							if (pointMsg.InCircle)
							{
								inCirclePoints++;
							}

							break;
						}
					case GetCurrentPiMsg _:
						{
							var pi = 4.0 * (double)inCirclePoints / (double)totalPoints;
							var err = Math.Abs(Math.PI - pi);

							sender.Tell(new CurrentPiMsg(pi, err));

							break;
						}
				}
			});
		}

		private static void GenerateRandomPoints(Actor actor)
		{
			var rnd = new Random(Environment.TickCount * Thread.CurrentThread.ManagedThreadId);

			while (true)
			{
				var x = rnd.NextDouble();
				var y = rnd.NextDouble();

				var r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

				var msg = new PointMsg(x, y, r <= 1);

				actor.Tell(msg);
			}
		}
    }
}
