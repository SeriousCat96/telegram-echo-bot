using EchoBot.Telegram.Actions;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageToBotActionFilter : MessageNotEmptyActionFilter
	{
		public override int Order => 20;

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.User, out var user);
			return base.IsAllowed(update, metadata)
					   && (user as User)?.Id == update.Message.ReplyToMessage?.From?.Id;
		}
	}
}
