using EchoBot.Telegram.Actions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class UserIsNotRepliedActionFilter : MessageNotEmptyActionFilter
	{
		private readonly ILogger<UserIsNotRepliedActionFilter> _logger;

		public override int Order => 30;

		public UserIsNotRepliedActionFilter(ILogger<UserIsNotRepliedActionFilter> logger)
		{
			_logger = logger;
		}

		public bool CheckCondition(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;

			if (!metadata.TryGetValue(MetadataKeys.RepliedUsers, out var userIds))
			{
				_logger.LogWarning($"metadata key {MetadataKeys.RepliedUsers} not found");
				userIds = new HashSet<long>();
			}

			var repliedUsersIds = (HashSet<long>)userIds;
			return !repliedUsersIds.Contains(message.From.Id);
		}
	}
}
