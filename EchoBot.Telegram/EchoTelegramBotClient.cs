using EchoBot.Core.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EchoBot.Telegram
{
	public class EchoTelegramBotClient : IEchoTelegramBotClient
	{
		private User _user;
		private readonly ILogger<EchoTelegramBotClient> _logger;
		private readonly BotOptions _options;
		private readonly TelegramBotClient _bot;

		public User User
		{
			get
			{
				EnsureUserExists();
				return _user;
			}
		}

		public EchoTelegramBotClient(
			ILogger<EchoTelegramBotClient> logger,
			BotOptions options)
		{
			_logger = logger;
			_options = options;
			_bot = new TelegramBotClient(_options.Token);
		}

		public Task<bool> TestApiAsync(CancellationToken cancellationToken = default)
		{
			return _bot.TestApiAsync(cancellationToken);
		}

		public Task<Message> SendMessageAsync(
			ChatId chatId,
			string text,
			ParseMode? parseMode = default,
			IEnumerable<MessageEntity> entities = default,
			bool? disableWebPagePreview = default,
			bool? disableNotification = default,
			int? replyToMessageId = default,
			bool? allowSendingWithoutReply = default,
			IReplyMarkup replyMarkup = default,
			CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Message ({0}): \"{1}\" to {2}", _user.Username, text, chatId);
			return _bot.SendTextMessageAsync(
				chatId,
				text,
				parseMode,
				entities,
				disableWebPagePreview,
				disableNotification,
				replyToMessageId,
				allowSendingWithoutReply,
				replyMarkup,
				cancellationToken);
		}

		public Task<ChatMember> GetChatMemberAsync(
			ChatId chatId,
			long userId,
			CancellationToken cancellationToken = default)
		{
			return _bot.GetChatMemberAsync(
				chatId,
				userId,
				cancellationToken);
		}

		public Task<User> GetMeAsync(CancellationToken cancellationToken = default)
		{
			return _bot.GetMeAsync(cancellationToken);
		}

		public Task<Update[]> GetUpdatesAsync(
			int? offset = default,
			int? limit = default,
			int? timeout = default,
			IEnumerable<UpdateType> allowedUpdates = default,
			CancellationToken cancellationToken = default)
		{
			return _bot.GetUpdatesAsync(offset, limit, timeout, allowedUpdates, cancellationToken);
		}

		private void EnsureUserExists()
		{
			if (_user == null)
			{
				_user = GetMeAsync()
					.ConfigureAwait(continueOnCapturedContext: false)
					.GetAwaiter()
					.GetResult();
			}
		}
	}
}
