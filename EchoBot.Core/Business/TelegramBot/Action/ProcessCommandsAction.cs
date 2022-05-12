using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Commands;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Action
{
	public class ProcessCommandsAction : ActionBase
	{
		private readonly IBotCommandRepository _commandRepository;

		public ProcessCommandsAction(
			IBotCommandRepository commandRepository,
			ILogger<ProcessCommandsAction> logger)
			: base(logger)
		{
			_commandRepository = commandRepository;
		}

		public override int Order => 10;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Break;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;
			if (message != null)
			{
				var command = _commandRepository.GetCommandByName(message.Text);
				if (command != null)
				{
					await command.ExecuteCommandAsync(message);
					return ActionResult.Succeed;
				}
			}

			return ActionResult.NotExecuted;
		}
	}
}
