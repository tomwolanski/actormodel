namespace ActorSystem.Core
{
	public abstract class ActorBase : SupervisingActorBase
	{
		public ActorBase(ActorContext context)
			: base(context.Self, context.Parent, context.MailboxReader, context.ExceptionCallback)
		{ }
	}
}
