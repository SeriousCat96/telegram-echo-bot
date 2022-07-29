using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram.Actions;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class UserExcludedFromReplyActionFilter : MessageNotEmptyActionFilter
	{
		private readonly IEchoChatsService _chatsService;

		public override int Order => 25;

		public UserExcludedFromReplyActionFilter(IEchoChatsService chatsService)
		{
			_chatsService = chatsService;
		}

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.BotId, out var botId);
			return base.IsAllowed(update, metadata)
				&& !_chatsService
						.GetExcludedUsers((int)botId)
						.Any(name => name == (update.Message.From.Username ?? update.Message.From.Id.ToString()));
		}
	}
}
