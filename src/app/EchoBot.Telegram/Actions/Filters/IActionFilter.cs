using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions.Filters
{
	public interface IActionFilter : ISortable
	{
		bool IsAllowed(Update update, Dictionary<string, object> metadata);
	}
}
