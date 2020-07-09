using ActorSystem.Core.Supervision;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace ActorSystem.Core
{
	public abstract class SupervisingActorBase : SupervisedActorBase
	{
		private readonly Dictionary<SupervisedActorBase, Func<SupervisedActorBase>> _children = new Dictionary<SupervisedActorBase, Func<SupervisedActorBase>>();

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
			_children.Add(child, childFactory);
			child.Start();

			return reference;
		}

		protected override void OnStopped()
		{
			_children.Keys.ForEach(child => child.Stop());
			base.OnStopped();
		}

		protected virtual ExceptionHandelingStrategy OnChildException(object message, Exception ex) => ExceptionHandelingStrategy.SkipMessage;

		private void ChildActorExceptionCallback(SupervisedActorBase child, object message, ActorRef sender, Exception ex)
		{
			var strategy = OnChildException(message, ex);

			switch (strategy)
			{
				case ExceptionHandelingStrategy.SkipMessage:
					{
						break;
					}
				case ExceptionHandelingStrategy.KillChild:
					{
						child.Stop();
						_children.Remove(child);
						break;
					}
				case ExceptionHandelingStrategy.RestartChild:
					{
						var childFactory = _children[child];
						child.Stop();
						_children.Remove(child);

						var newChild = childFactory();
						_children.Add(newChild, childFactory);
						newChild.HandleMessage(message, sender);
						newChild.Start();

						break;
					}
			}
		}
	}
}
