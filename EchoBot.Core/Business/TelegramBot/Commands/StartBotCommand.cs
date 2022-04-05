using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Commands
{
	[BotCommand("/start")]
	[BotCommand("/help")]
	public class StartBotCommand : IBotCommand
	{
		private readonly IEchoTelegramBotClient _botClient;
		private readonly CommandsOptions _commandOptions;

		public StartBotCommand(
			IEchoTelegramBotClient botClient,
			IOptions<CommandsOptions> commandOptions)
		{
			_botClient = botClient;
			_commandOptions = commandOptions.Value;
		}

		public Task ExecuteCommandAsync(Message message)
		{
			var text = string.Join(Environment.NewLine, _commandOptions.Info);

			return _botClient.SendMessageAsync(message.Chat, text);
		}
	}
}
