﻿using Microsoft.Extensions.Options;
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
		private readonly TelegramBotOptions _options;
		private readonly TelegramBotClient _bot;

		public EchoTelegramBotClient(IOptions<TelegramBotOptions> options)
		{
			_options = options.Value;
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
	}
}