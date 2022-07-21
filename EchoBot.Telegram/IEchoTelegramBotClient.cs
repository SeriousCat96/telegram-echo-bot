using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EchoBot.Telegram
{
	public interface IEchoTelegramBotClient
	{
		User User { get; }

		Task<User> GetMeAsync(CancellationToken cancellationToken = default);

		Task<Update[]> GetUpdatesAsync(
			int? offset = default,
			int? limit = default,
			int? timeout = default,
			IEnumerable<UpdateType> allowedUpdates = default,
			CancellationToken cancellationToken = default);

		Task<Message> SendMessageAsync(
			ChatId chatId,
			string text,
			ParseMode? parseMode = default,
			IEnumerable<MessageEntity> entities = default,
			bool? disableWebPagePreview = default,
			bool? disableNotification = default,
			int? replyToMessageId = default,
			bool? allowSendingWithoutReply = default,
			IReplyMarkup replyMarkup = default,
			CancellationToken cancellationToken = default);

		Task<ChatMember> GetChatMemberAsync(
			ChatId chatId,
			long userId,
			CancellationToken cancellationToken = default);

		Task<bool> TestApiAsync(CancellationToken cancellationToken = default);
	}
}
