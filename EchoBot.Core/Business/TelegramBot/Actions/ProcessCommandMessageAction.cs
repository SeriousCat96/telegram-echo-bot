using EchoBot.Core.Business.TelegramBot.Actions.Filters;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Actions.Filters;
using EchoBot.Telegram.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions
{
	[ActionFilter(typeof(MessageNotEmptyActionFilter))]
	public class ProcessCommandMessageAction : ActionBase
	{
		private readonly IBotCommandRepository _commandRepository;

		public ProcessCommandMessageAction(
			IBotCommandRepository commandRepository,
			IServiceProvider serviceProvider,
			ILogger<ProcessCommandMessageAction> logger)
			: base(serviceProvider, logger)
		{
			_commandRepository = commandRepository;
		}

		public override int Order => 10;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Break;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;
			var command = _commandRepository.GetCommandByName(message.Text);

			if (command != null)
			{
				await command.ExecuteCommandAsync(message);
				return ActionResult.Succeed;
			}

			return ActionResult.NotExecuted;
		}
	}
}
