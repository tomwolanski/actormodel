using System;
using ActorSystem.PiCalculator.Actors;

namespace ActorSystem.PiCalculator
{
    static class Program
    {
        static void Main(string[] args)
        {
            var system = new Core.ActorSystem();

            var piCalculator = system.Create("calc", ctx => new PiCalculatingActor(ctx, 10));

            var logger = system.Create("log", ctx => new ConsoleLogActor(ctx, Console.Out, piCalculator));

            Console.ReadLine();
        }
    }
}
