using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TelegramBot.Actions.Filters;
using EchoBot.Telegram;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Actions.Filters;
using EchoBot.Telegram.Engine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Actions
{
	[ActionFilter(typeof(MessageHasSenderActionFilter))]
	[ActionFilter(typeof(UserExcludedFromReplyActionFilter))]
	[ActionFilter(typeof(UserIsNotRepliedActionFilter))]
	[ActionFilter(typeof(MessageReplyFrequencyActionFilter))]
	public class ProcessReplyMessageAction : ActionBase
	{
		private readonly ILogger<ProcessReplyMessageAction> _logger;
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly IEchoChatsService _chatsService;

		public ProcessReplyMessageAction(
			ITelegramBotInstanceRepository botInstanceRepository,
			IEchoChatsService chatsService,
			IServiceProvider serviceProvider,
			ILogger<ProcessReplyMessageAction> logger)
			: base(serviceProvider, logger)
		{
			_botInstanceRepository = botInstanceRepository;
			_chatsService = chatsService;
			_logger = logger;
		}

		public override int Order => 25;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Continue;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			var message = update.Message;

			if (!metadata.TryGetValue(MetadataKeys.RepliedUsers, out var userIds))
			{
				_logger.LogWarning($"metadata key {MetadataKeys.RepliedUsers} not found");
				userIds = new HashSet<long>();
			}

			var botId = GetBotId(metadata);
			var repliedUsersIds = (HashSet<long>)userIds;
			var replyMessage = await _chatsService.GetRandomMessageAsync(botId);
			var botInstance = _botInstanceRepository.GetInstance(botId);

			await botInstance.Client.SendMessageAsync(
				message.Chat,
				replyMessage.Text,
				replyToMessageId: message.MessageId,
				parseMode: ParseMode.Markdown);

			repliedUsersIds.Add(message.From.Id);

			return ActionResult.Succeed;
		}
	}
}

