using System;

namespace ActorSystem.Core.Supervision
{
	public delegate void ActorExceptionCallback(SupervisedActorBase actor,	object message, ActorRef sender, Exception exception);
}
