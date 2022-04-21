using Microsoft.Extensions.Options;
using System;

namespace EchoBot.Core.Business
{
	public class EchoChatsService : IEchoChatsService
	{
		private readonly Random _rnd;
		private readonly ITemplateMessageParser _templateParser;
		private readonly EchoChatOptions _options;
		private uint _counter = 0;

		public EchoChatsService(
			ITemplateMessageParser templateParser,
			IOptions<EchoChatOptions> options)
		{
			_templateParser = templateParser;
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

			var text = _options.Messages[_rnd.Next(from, to)];
			return _templateParser.ParseTemplate(text);
		}

		public bool FrequencyCheck()
		{
			return _counter++ % _options.ReplyFrequency == 0;
		}
	}
}
