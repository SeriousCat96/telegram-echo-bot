using EchoBot.Core.Business.TelegramBot.Models;
using EchoBot.Core.Options;
using EchoBot.Telegram;
using EchoBot.Telegram.Engine;
using EchoBot.Telegram.Options;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.TemplateParser
{
	public class TemplateMessageParser : ITemplateMessageParser
	{
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly BotsOptions _options;
		private readonly Random _rnd;

		public TemplateMessageParser(
			ITelegramBotInstanceRepository botInstanceRepository,
			IOptions<BotsOptions> options)
		{
			_botInstanceRepository = botInstanceRepository;
			_options = options.Value;
			_rnd = new Random();
		}

		public async Task<TelegramMessage> ParseTemplateAsync(string template, int botId)
		{
			if (string.IsNullOrWhiteSpace(template))
			{
				return null;
			}

			if (!template.Contains("<username>"))
			{
				return new TelegramMessage
				{
					Text = template
				};
			}

			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var chatOptions = botOptions.ChatOptions;
			var templateOptions = botOptions.Templates;

			var users = chatOptions.Users;

			int from = 0;
			int to = users.Length;
			string username;

			if (users.Length == 0)
			{
				username = templateOptions.Fallbacks.Username;
			}
			else
			{
				username = users[_rnd.Next(from, to)];
			}

			if (!long.TryParse(username, out long userId))
			{
				return new TelegramMessage
				{
					Text = template.Replace("<username>", "@" + username)
				};
			}

			var botInstance = _botInstanceRepository.GetInstance(botId);

			var chatMember = await botInstance.Client.GetChatMemberAsync(chatOptions.ChatId, userId);
			var user = chatMember.User;
			username = user.FirstName;
			
			return new TelegramMessage
			{
				Text = template.Replace("<username>", $"[{username}](tg://user?id={userId})"),
			};
		}
	}
}
