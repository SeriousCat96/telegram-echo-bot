using Microsoft.Extensions.Options;
using System;

namespace EchoBot.Core
{
	public class EchoChatsService : IEchoChatsService
	{
		private readonly Random _rnd;
		private readonly EchoChatOptions _options;
		private uint _counter = 0;

		public EchoChatsService(IOptions<EchoChatOptions> options)
		{
			_options = options.Value;
			_rnd = new Random();
		}

		public string[] GetUsers()
		{
			return _options.Users;
		}

		public string GetRandomMessage()
		{
			int from = 0;
			int to = _options.Messages.Length - 1;

			return _options.Messages[_rnd.Next(from, to)];
		}

		public bool FrequencyCheck()
		{
			return _counter++ % _options.ReplyFrequency == 0;
		}
	}
}
