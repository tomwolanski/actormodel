using ActorSystem.Core.Supervision;
using System;
using System.Threading.Channels;

namespace ActorSystem.Core
{
	public abstract class SupervisedActorBase : MailboxProcessingActorBase
	{
		private readonly ActorExceptionCallback _exceptionCallback;

		public ActorRef Parent { get; }

		protected SupervisedActorBase(ActorRef self, ActorRef parent, ChannelReader<(object, ActorRef)> mailbox, ActorExceptionCallback exceptionCallback)
			: base(self, mailbox)
		{
			_exceptionCallback = exceptionCallback;
			Parent = parent;
		}

		protected internal override sealed void HandleMessage(object message, ActorRef sender)
		{
			try
			{
				base.HandleMessage(message, sender);
			}
			catch (Exception e)
			{
				_exceptionCallback(this, message, sender, e);
			}
		}
	}
}
