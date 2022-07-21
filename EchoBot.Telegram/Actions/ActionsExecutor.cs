using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions
{
	public class ActionsExecutor : IActionsExecutor
	{
		private readonly IEnumerable<IAction> _actions;

		public ActionsExecutor(IEnumerable<IAction> actions)
		{
			_actions = actions;
		}

		public async Task ExecuteAsync(Update update, Dictionary<string, object> metadata)
		{
			
			var actions = _actions.OrderBy(action => action.Order);

			foreach (var action in actions)
			{
				var actionResult = await action.ExecuteAsync(update, metadata);
				if (actionResult != ActionResult.NotExecuted && action.PipelineBehavior == ActionPipelineBehavior.Break)
				{
					break;
				}
			}
		}
	}
}
