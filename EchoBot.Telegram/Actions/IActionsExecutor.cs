using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions
{
	public interface IActionsExecutor
	{
		Task ExecuteAsync(Update update, Dictionary<string, object> metadata);
	}
}
