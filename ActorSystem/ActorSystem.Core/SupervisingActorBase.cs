using ActorSystem.Core.Supervision;
using System;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ActorSystem.Core
{
	public abstract class SupervisingActorBase : SupervisedActorBase
	{
		private readonly ConcurrentDictionary<SupervisedActorBase, Func<SupervisedActorBase>> _children = new ConcurrentDictionary<SupervisedActorBase, Func<SupervisedActorBase>>();

		protected SupervisingActorBase(ActorRef self, ActorRef parent, ChannelReader<(object, ActorRef)> mailbox, ActorExceptionCallback exceptionCallback)
			: base(self, parent, mailbox, exceptionCallback)
		{ }

		protected ActorRef Create<TActor>(string name, Func<ActorContext, TActor> actorBuilder) where TActor : ActorBase
		{
			var id = $"{Self.Id}.{name}";
			var mailbox = Channel.CreateUnbounded<(object, ActorRef)>(new UnboundedChannelOptions { SingleReader = true });
			var reference = new ActorRef(id, mailbox.Writer);

			var ctx = new ActorContext(id, reference, Self, mailbox.Reader, ChildActorExceptionCallback);
			var childFactory = new Func<SupervisedActorBase>(() => actorBuilder(ctx));

			var child = childFactory();
			_children.TryAdd(child, childFactory);
			child.Start();

			return reference;
		}

		protected override void OnStopped()
		{
			_children.ForEach(child => child.Key.Stop());
			base.OnStopped();
		}

		protected virtual ExceptionHandlingStrategy OnChildException(object message, Exception ex) => ExceptionHandlingStrategy.SkipMessage;

		private void ChildActorExceptionCallback(SupervisedActorBase child, object message, ActorRef sender, Exception ex)
		{
			var childId = child.Self.Id;
			var strategy = OnChildException(message, ex);

			switch (strategy)
			{
				case ExceptionHandlingStrategy.SkipMessage:
					{
						break;
					}
				case ExceptionHandlingStrategy.KillChild:
					{
						child.Stop();
						_children.TryRemove(child, out var _);
						break;
					}
				case ExceptionHandlingStrategy.RestartChild:
					{
						child.Stop();
						_children.TryRemove(child, out var childFactory);

						var newChild = childFactory();
						_children.TryAdd(newChild, childFactory);
						newChild.HandleMessage(message, sender);
						newChild.Start();

						break;
					}
			}
		}
	}
}
