﻿using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ActorSystem.Core
{
	public abstract class MailboxProcessingActorBase
	{
		private readonly ChannelReader<(object, ActorRef)> _mailbox;
		private readonly CancellationTokenSource _cancelTokenSrc = new CancellationTokenSource();

		public ActorRef Self { get; }

		protected MailboxProcessingActorBase(ActorRef self, ChannelReader<(object, ActorRef)> mailbox)
		{
			_mailbox = mailbox;
			Self = self;
		}

		public void Start() => Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);

		public void Stop() => _cancelTokenSrc.Cancel();

		protected abstract void OnMessage(object message, ActorRef sender);

		protected virtual void OnStarted() => Console.WriteLine($"{Self.Id} started");

		protected virtual void OnStopped() => Console.WriteLine($"{Self.Id} stopped");

		protected internal virtual void HandleMessage(object message, ActorRef sender) => OnMessage(message, sender);

		private async void Loop()
		{
			OnStarted();

			while( !_cancelTokenSrc.Token.IsCancellationRequested )
			{
				var (message, sender) = await _mailbox.ReadAsync(_cancelTokenSrc.Token);
				HandleMessage(message, sender);
			}

			OnStopped();
		}
	}
}
