using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram;
using EchoBot.Telegram.Actions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Actions
{
	public class ProcessReplyMessageAction : ActionBase
	{
		private readonly ILogger<ProcessReplyMessageAction> _logger;
		private readonly IEchoTelegramBotClient _botClient;
		private readonly IEchoChatsService _chatsService;

		public ProcessReplyMessageAction(
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService,
			ILogger<ProcessReplyMessageAction> logger)
			: base(logger)
		{
			_botClient = botClient;
			_chatsService = chatsService;
			_logger = logger;
		}

		public override int Order => 25;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Continue;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;

			if (message != null)
			{
				if (!metadata.TryGetValue(MetadataKeys.RepliedUsers, out var userIds))
				{
					_logger.LogWarning($"metadata key {MetadataKeys.RepliedUsers} not found");
					userIds = new HashSet<long>();
				}

				var repliedUsersIds = (HashSet<long>)userIds;

				if (!repliedUsersIds.Contains(message.From.Id) &&
					message.From != null &&
					(message.Chat.Type == ChatType.Private || _chatsService.FrequencyCheck()))
				{
					var excludedUsers = _chatsService.GetExcludedUsers();
					string username = message.From.Username ?? message.From.Id.ToString();

					if (!excludedUsers.Any(name => name == username))
					{
						var replyMessage = await _chatsService.GetRandomMessageAsync();
						await _botClient.SendMessageAsync(
							message.Chat,
							replyMessage.Text,
							replyToMessageId: message.MessageId,
							parseMode: ParseMode.Markdown);

						repliedUsersIds.Add(message.From.Id);

						return ActionResult.Succeed;
					}
				}
			}

			return ActionResult.NotExecuted;
		}
	}
}

