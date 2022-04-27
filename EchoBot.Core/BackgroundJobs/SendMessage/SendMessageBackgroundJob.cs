using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.BackgroundJobs.SendMessage
{
	public class SendMessageBackgroundJob
	{
		private readonly EchoChatOptions _options;
		private readonly IEchoTelegramBotClient _botClient;
		private readonly IEchoChatsService _chatsService;

		public SendMessageBackgroundJob(
			IOptions<EchoChatOptions> options,
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService)
		{
			_options = options.Value;
			_botClient = botClient;
			_chatsService = chatsService;
		}

		public async Task ExecuteAsync()
		{
			var message = await _chatsService.GetRandomMessageAsync();

			ChatId chat;
			if (long.TryParse(_options.ChatId, out long chatId))
			{
				chat = chatId;
			}
			else
			{
				chat = _options.ChatId;
			}

			await _botClient.SendMessageAsync(
				chat,
				message.Text,
				parseMode: ParseMode.Markdown);
		}
	}
}
