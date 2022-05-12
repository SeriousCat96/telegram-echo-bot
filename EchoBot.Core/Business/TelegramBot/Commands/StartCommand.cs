using EchoBot.Core.Business.Commands;
using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Commands
{
	[BotCommand("/start", "standard bot command")]
	[BotCommand("/help", "shows list of commands")]
	public class StartCommand : IBotCommand
	{
		private readonly IEchoTelegramBotClient _botClient;

		public StartCommand(IEchoTelegramBotClient botClient)
		{
			_botClient = botClient;
		}

		public Task ExecuteCommandAsync(Message message)
		{
			var cmds = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(IBotCommand).IsAssignableFrom(type))
				.SelectMany(type => type.GetCustomAttributes<BotCommandAttribute>())
				.Select(attr => new CommandInfo
				{
					Command = attr.CommandName,
					Description = attr.Description
				});

			var text = string.Join(Environment.NewLine, cmds);

			return _botClient.SendMessageAsync(message.Chat, text, parseMode: ParseMode.Markdown);
		}
	}
}
