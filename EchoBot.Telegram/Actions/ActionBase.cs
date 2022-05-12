using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions
{
	public abstract class ActionBase : IAction
	{
		private readonly ILogger<ActionBase> _logger;

		protected ActionBase(ILogger<ActionBase> logger)
		{
			_logger = logger;
		}

		public abstract int Order { get; }

		public abstract ActionPipelineBehavior PipelineBehavior { get; }

		public async Task<ActionResult> ExecuteAsync(Update update, Dictionary<string, object> metadata)
		{
			try
			{
				return await ExecuteCoreAsync(update, metadata);
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, $"Action {GetType().Name} completed with error");
				return ActionResult.Failed;
			}
		}

		public abstract Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata);
	}
}
