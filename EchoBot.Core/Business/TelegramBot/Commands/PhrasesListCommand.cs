using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Commands
{
	[BotCommand("/phrases", "shows list of phrases")]
	public class PhrasesListCommand : IBotCommand
	{
		private readonly IEchoTelegramBotClient _botClient;
		private readonly EchoChatOptions _chatOptions;
		public PhrasesListCommand(
			IEchoTelegramBotClient botClient,
			IOptions<EchoChatOptions> chatOptions)
		{
			_botClient = botClient;
			_chatOptions = chatOptions.Value;
		}

		public async Task ExecuteCommandAsync(Message message)
		{
			const int maxMessageLength = 4096;

			var messages = _chatOptions.Messages;
			var phrases = new List<string>();
			int totalLength = 0;
			int newLineLength = Environment.NewLine.Length;

			for (int i = 0; i < messages.Length; ++i)
			{
				var phrase = $"{i + 1}. {messages[i]}";
				
				phrases.Add(phrase);
				totalLength += phrase.Length + newLineLength;

				if (i < messages.Length - 1)
				{
					var nextPhrase = $"{i + 2}. {messages[i + 1]}";
					if (totalLength + nextPhrase.Length + newLineLength > maxMessageLength)
					{
						await _botClient.SendMessageAsync(
							message.Chat,
							string.Join(Environment.NewLine, phrases),
							parseMode: ParseMode.Markdown);

						phrases.Clear();
						totalLength = 0;
					}
					
					phrases.Add(nextPhrase);
					totalLength += nextPhrase.Length + newLineLength;
					++i;
				}
			}

			if (phrases.Count > 0)
			{
				await _botClient.SendMessageAsync(
					message.Chat,
					string.Join(Environment.NewLine, phrases),
					parseMode: ParseMode.Markdown);
			}
		}
	}
}
