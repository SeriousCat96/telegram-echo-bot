using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Users;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Actions
{
	public class ProcessReplyToBotMessageAction : ActionBase
	{
		private readonly ILogger<ProcessReplyToBotMessageAction> _logger;
		private readonly IEchoTelegramBotClient _botClient;
		private readonly IEchoChatsService _chatsService;
		private readonly ICurrentUser _currentUser;

		public ProcessReplyToBotMessageAction(
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService,
			ICurrentUser currentUser,
			ILogger<ProcessReplyToBotMessageAction> logger)
			: base(logger)
		{
			_botClient = botClient;
			_chatsService = chatsService;
			_currentUser = currentUser;
			_logger = logger;
		}

		public override int Order => 20;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Break;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;
			var currentUser = _currentUser.User;

			if (message != null && currentUser != null)
			{
				if (!metadata.TryGetValue(MetadataKeys.RepliedUsers, out var userIds))
				{
					_logger.LogWarning($"metadata key {MetadataKeys.RepliedUsers} not found");
					userIds = new HashSet<long>();
				}

				var repliedUsersIds = (HashSet<long>)userIds;

				if (!repliedUsersIds.Contains(message.From.Id) &&
					message.From != null &&
					message.ReplyToMessage != null &&
					message.ReplyToMessage.From?.Id == currentUser.Id)
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

			return ActionResult.NotExecuted;
		}
	}
}
