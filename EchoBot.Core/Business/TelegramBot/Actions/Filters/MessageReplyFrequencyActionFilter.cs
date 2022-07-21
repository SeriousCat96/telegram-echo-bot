using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram.Actions;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageReplyFrequencyActionFilter : MessageNotEmptyActionFilter
	{
		private readonly IEchoChatsService _chatsService;

		public override int Order => 40;

		public MessageReplyFrequencyActionFilter(IEchoChatsService chatsService)
		{
			_chatsService = chatsService;
		}

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.BotId, out var botId);
			return base.IsAllowed(update, metadata) && update.Message.Chat.Type == ChatType.Private
				|| _chatsService.FrequencyCheck((int)botId);
		}
	}
}
