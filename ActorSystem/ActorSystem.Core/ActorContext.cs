using ActorSystem.Core.Supervision;
using System.Threading.Channels;

namespace ActorSystem.Core
{
	public class ActorContext
	{
		public string Id { get; }
		public ActorRef Self { get; }
		public ActorRef Parent { get; }
		public ChannelReader<(object, ActorRef)> MailboxReader { get; }
		public ActorExceptionCallback ExceptionCallback { get; }

		public ActorContext(string id, ActorRef self, ActorRef parent, ChannelReader<(object, ActorRef)> mailboxReader, ActorExceptionCallback exceptionCallback)
		{
			Id = id;
			Self = self;
			Parent = parent;
			MailboxReader = mailboxReader;
			ExceptionCallback = exceptionCallback;
		}
	}
}
