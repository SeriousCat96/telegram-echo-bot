using EchoBot.Telegram.Actions.Filters;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageNotEmptyActionFilter : IActionFilter
	{
		public virtual int Order => 10;

		public virtual bool IsAllowed(Update update, Dictionary<string, object> metadata) => update.Message != null;
	}
}
