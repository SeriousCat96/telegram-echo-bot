using EchoBot.Telegram.Actions.Filters;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageHasSenderActionFilter : MessageNotEmptyActionFilter
	{
		public override int Order => 15;

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata) 
			=> base.IsAllowed(update, metadata) && update.Message.From != null;
	}
}
