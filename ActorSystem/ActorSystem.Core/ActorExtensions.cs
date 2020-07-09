using System.Threading.Tasks;

namespace ActorSystem.Core
{

	public static class ActorExtensions
	{
		public static void ScheduleMessage(this ActorRef actor, int period, object message, ActorRef sender)
		{
			Task.Delay(period).ContinueWith(_ => actor.Tell(message, sender));
		}
	}
}
