using SimpleActor.Core;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SimpleActor.HelloWorld
{
    static class Program
    {
        public static async Task Main()
		{
			var actor = GetActor();

			actor.Tell("Hello ");
			actor.Tell("World!");

			var result = await actor.AskAsync<string>(new GetStateMsg());
			Console.WriteLine(result);

			actor.GracefulStop();
		}

		private static Actor GetActor()
		{
			var state = new StringBuilder();

			return new Actor((msg, sender) =>
			{
				switch (msg)
				{
					case GetStateMsg getStateMsg:
						{
							var snapshot = state.ToString();
							sender.Tell(snapshot);
							break;
						}
					case string str:
						{
							state.Append(str);
							break;
						}
				}
			});
		}
    }
}
