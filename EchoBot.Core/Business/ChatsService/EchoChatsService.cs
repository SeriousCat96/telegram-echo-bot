using EchoBot.Core.Business.TelegramBot.Models;
using EchoBot.Core.Business.TemplateParser;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.ChatsService
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

		public string[] GetExcludedUsers()
		{
			return _options.ExcludedUsers;
		}

		public string[] GetUsers()
		{
			return _options.Users;
		}

		public async Task<TelegramMessage> GetRandomMessageAsync()
		{
			int from = 0;
			int to = _options.Messages.Length - 1;

			var text = _options.Messages[_rnd.Next(from, to)];
			return await _templateParser.ParseTemplateAsync(text);
		}

		public bool FrequencyCheck()
		{
			return _counter++ % _options.ReplyFrequency == 0;
		}
	}
}
