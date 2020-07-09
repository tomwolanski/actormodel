using System.Threading.Channels;

namespace ActorSystem.Core
{
	public sealed class ActorRef
	{
		public static ActorRef Empty { get; } = new ActorRef(null, null);

		private readonly ChannelWriter<(object, ActorRef)> _mailboxWriter;

		public string Id { get; }

		public ActorRef(string id, ChannelWriter<(object, ActorRef)> mailboxWritter)
		{
			_mailboxWriter = mailboxWritter;
			Id = id;
		}

		public void Tell(object message, ActorRef sender) => _mailboxWriter?.TryWrite((message, sender));
	}
}
