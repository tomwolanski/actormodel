using System.Threading.Tasks;

namespace SimpleActor.Core
{
	public static class ActorExtensions
	{
		public static Task<TResponseMessage> AskAsync<TResponseMessage>(this Actor actor, object message)
		{
			var tcs = new TaskCompletionSource<TResponseMessage>();

			Actor tempAct = null;
			tempAct = new Actor((msg, _) =>
			{
				var value = (TResponseMessage)msg;
				tcs.SetResult(value);
				tempAct.GracefulStop();
			});

			actor.Tell(message, tempAct);

			return tcs.Task;
		}
	}
}