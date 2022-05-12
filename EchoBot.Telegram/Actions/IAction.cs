using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions
{
	public interface IAction
	{
		int Order { get; }

		ActionPipelineBehavior PipelineBehavior { get; }

		Task<ActionResult> ExecuteAsync(Update update, Dictionary<string, object> metadata);
	}
}
