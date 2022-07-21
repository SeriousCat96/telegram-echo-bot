using EchoBot.Core.Business.TelegramBot.Models;
using EchoBot.Core.Business.TemplateParser;
using EchoBot.Core.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.ChatsService
{
	public class EchoChatsService : IEchoChatsService
	{
		private readonly Random _rnd;
		private readonly ITemplateMessageParser _templateParser;
		private readonly BotsOptions _options;
		private Dictionary<int, uint> _counters;

		public EchoChatsService(
			ITemplateMessageParser templateParser,
			IOptions<BotsOptions> options)
		{
			_templateParser = templateParser;
			_options = options.Value;
			_rnd = new Random();
			_counters = options.Value.Bots
				.Select(bot => bot.Id)
				.ToDictionary(k => k, v => (uint)0);
		}

		public string[] GetExcludedUsers(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var chatOptions = botOptions.ChatOptions;

			return chatOptions.ExcludedUsers;
		}

		public string[] GetUsers(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var chatOptions = botOptions.ChatOptions;

			return chatOptions.Users;
		}

		public async Task<TelegramMessage> GetRandomMessageAsync(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var chatOptions = botOptions.ChatOptions;

			int from = 0;
			int to = chatOptions.Messages.Length;

			var text = chatOptions.Messages[_rnd.Next(from, to)];
			return await _templateParser.ParseTemplateAsync(text, botId);
		}

		public bool FrequencyCheck(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var chatOptions = botOptions.ChatOptions;

			return _counters[botId]++ % chatOptions.ReplyFrequency == 0;
		}
	}
}
