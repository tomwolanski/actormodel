using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleActor.Core
{
	public sealed class Actor
	{
		private readonly Channel<(object, Actor)> _mailbox = Channel.CreateUnbounded<(object, Actor)>();

		private readonly Action<object, Actor> _handler;

		public Actor(Action<object, Actor> handler)
		{
			_handler = handler;

			Task.Run(Loop);
		}

		public void Tell(object message, Actor sender = null)
		{
			var pair = (message, sender);
			_mailbox.Writer.TryWrite(pair);
		}

		public void GracefulStop() => _mailbox.Writer.Complete();

		private async Task Loop()
		{
			await foreach (var pair in _mailbox.Reader.ReadAllAsync())
			{
				var (message, sender) = pair;
				_handler(message, sender);
			}
		}
	}
}
