using EchoBot.Telegram.Actions.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Actions
{
	public abstract class ActionBase : IAction
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<ActionBase> _logger;

		protected ActionBase(
			IServiceProvider serviceProvider,
			ILogger<ActionBase> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public abstract int Order { get; }

		public abstract ActionPipelineBehavior PipelineBehavior { get; }

		public async Task<ActionResult> ExecuteAsync(Update update, Dictionary<string, object> metadata)
		{
			try
			{
				var actionFilters = GetType()
					.GetCustomAttributes<ActionFilterAttribute>()
					.Select(f => f.FilterType)
					.Select(type => (IActionFilter)_serviceProvider.GetRequiredService(type))
					.OrderBy(filter => filter.Order);

				foreach (var actionFilter in actionFilters)
				{
					if (!actionFilter.IsAllowed(update, metadata))
					{
						return ActionResult.NotExecuted;
					}
				}

				return await ExecuteCoreAsync(update, metadata);
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, $"Action {GetType().Name} completed with error");
				return ActionResult.Failed;
			}
		}

		public abstract Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata);

		protected User GetCurrentUser(Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.User, out var user);
			return (User)user;
		}
		
		protected int GetBotId(Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.BotId, out var botId);
			return (int)botId;
		}
	}
}
