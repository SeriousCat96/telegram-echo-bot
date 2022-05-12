using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TelegramBot.Models;
using EchoBot.Telegram;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.TemplateParser
{
	public class TemplateMessageParser : ITemplateMessageParser
	{
		private readonly IEchoTelegramBotClient _botClient;
		private readonly EchoChatOptions _chatOptions;
		private readonly TemplateOptions _templateOptions;
		private readonly Random _rnd;

		public TemplateMessageParser(
			IEchoTelegramBotClient botClient,
			IOptions<EchoChatOptions> chatOptions,
			IOptions<TemplateOptions> templateOptions)
		{
			_botClient = botClient;
			_chatOptions = chatOptions.Value;
			_templateOptions = templateOptions.Value;
			_rnd = new Random();
		}

		public async Task<TelegramMessage> ParseTemplateAsync(string template)
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

			var users = _chatOptions.Users;

			int from = 0;
			int to = users.Length - 1;
			string username;

			if (users.Length == 0)
			{
				username = _templateOptions.Fallbacks.Username;
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

			var chatMember = await _botClient.GetChatMemberAsync(_chatOptions.ChatId, userId);
			var user = chatMember.User;
			username = user.FirstName;
			
			return new TelegramMessage
			{
				Text = template.Replace("<username>", $"[{username}](tg://user?id={userId})"),
			};
		}
	}
}
