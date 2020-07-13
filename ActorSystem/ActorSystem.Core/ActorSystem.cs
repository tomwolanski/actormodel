using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace ActorSystem.Core
{
	public class ActorSystem
	{
		private readonly List<SupervisedActorBase> _children = new List<SupervisedActorBase>();

		public ActorRef Create<TActor>(string name, Func<ActorContext, TActor> actorBuilder)
		where TActor : ActorBase
		{
			var id = $"$.{name}";
			var mailbox = Channel.CreateUnbounded<(object, ActorRef)>();
			var reference = new ActorRef(id, mailbox.Writer);
			var ctx = new ActorContext(id, reference, ActorRef.Empty, mailbox.Reader, ChildActorExceptionCallback);

			var child = actorBuilder(ctx);
			_children.Add(child);
			child.Start();

			return reference;
		}

		private void ChildActorExceptionCallback( SupervisedActorBase child, object message, ActorRef sender, Exception ex)
		{
			child.Stop();
			_children.Remove(child);
		}
	}
}
