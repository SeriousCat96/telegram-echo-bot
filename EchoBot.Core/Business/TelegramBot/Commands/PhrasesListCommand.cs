using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using EchoBot.Telegram.Engine;
using EchoBot.Telegram.Options;
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
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly EchoChatOptions _chatOptions;
		public PhrasesListCommand(
			ITelegramBotInstanceRepository botInstanceRepository,
			IOptions<EchoChatOptions> chatOptions)
		{
			_botInstanceRepository = botInstanceRepository;
			_chatOptions = chatOptions.Value;
		}

		public async Task ExecuteCommandAsync(Message message, int botId)
		{
			const int maxMessageLength = 4096;

			var messages = _chatOptions.Messages;
			var phrases = new List<string>();
			int totalLength = 0;
			int newLineLength = Environment.NewLine.Length;
			var botInstance = _botInstanceRepository.GetInstance(botId);

			for (int i = 0; i < messages.Length; ++i)
			{
				var phrase = $"{i + 1}. {messages[i]}";
				
				if (totalLength + phrase.Length + newLineLength > maxMessageLength)
				{
					await botInstance.Client.SendMessageAsync(
						message.Chat,
						string.Join(Environment.NewLine, phrases),
						parseMode: ParseMode.Markdown);

					phrases.Clear();
					totalLength = 0;
				}

				phrases.Add(phrase);
				totalLength += phrase.Length + newLineLength;
			}

			if (phrases.Count > 0)
			{
				await botInstance.Client.SendMessageAsync(
					message.Chat,
					string.Join(Environment.NewLine, phrases),
					parseMode: ParseMode.Markdown);
			}
		}
	}
}
