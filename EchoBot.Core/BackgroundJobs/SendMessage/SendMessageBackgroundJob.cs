using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Options;
using EchoBot.Telegram.Engine;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.BackgroundJobs.SendMessage
{
	public class SendMessageBackgroundJob
	{
		private readonly BotsOptions _options;
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly IEchoChatsService _chatsService;

		public SendMessageBackgroundJob(
			IOptions<BotsOptions> options,
			ITelegramBotInstanceRepository botInstanceRepositoryt,
			IEchoChatsService chatsService)
		{
			_options = options.Value;
			_botInstanceRepository = botInstanceRepositoryt;
			_chatsService = chatsService;
		}

		public async Task ExecuteAsync(int botId, CancellationToken cancellationToken = default)
		{
			var message = await _chatsService.GetRandomMessageAsync(botId);
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();

			if (botOptions == null)
			{
				return;
			}

			ChatId chat;
			if (long.TryParse(botOptions.ChatOptions.ChatId, out long chatId))
			{
				chat = chatId;
			}
			else
			{
				chat = botOptions.ChatOptions.ChatId;
			}

			var botInstance = _botInstanceRepository.GetInstance(botId);

			await botInstance.Client.SendMessageAsync(
				chat,
				message.Text,
				parseMode: ParseMode.Markdown,
				cancellationToken: cancellationToken);
		}
	}
}
