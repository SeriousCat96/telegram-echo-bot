using EchoBot.Core.Business;
using EchoBot.Telegram;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EchoBot.Core.BackgroundJobs.SendMessage
{
	public class SendMessageBackgroundJob
	{
		private readonly SendMessageOptions _options;
		private readonly IEchoTelegramBotClient _botClient;
		private readonly IEchoChatsService _chatsService;

		public SendMessageBackgroundJob(
			IOptions<SendMessageOptions> options,
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService)
		{
			_options = options.Value;
			_botClient = botClient;
			_chatsService = chatsService;
		}

		public async Task ExecuteAsync()
		{
			var message = _chatsService.GetRandomMessage();
			await _botClient.SendMessageAsync(_options.ChatId, message);
		}
	}
}
