using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TelegramBot.Actions.Filters;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Actions.Filters;
using EchoBot.Telegram.Engine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions
{
	[ActionFilter(typeof(MessageHasSenderActionFilter))]
	[ActionFilter(typeof(UserExcludedFromReplyActionFilter))]
	[ActionFilter(typeof(UserIsNotRepliedActionFilter))]
	[ActionFilter(typeof(MessageReplyFrequencyActionFilter))]
	public class ProcessReplyVideoAction : ActionBase
	{
		private readonly ILogger<ProcessReplyToBotVideoAction> _logger;
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly IVideoService _videoService;

		public ProcessReplyVideoAction(
			ITelegramBotInstanceRepository botInstanceRepository,
			IVideoService videoService,
			IServiceProvider serviceProvider,
			ILogger<ProcessReplyToBotVideoAction> logger)
			: base(serviceProvider, logger)
		{
			_botInstanceRepository = botInstanceRepository;
			_videoService = videoService;
			_logger = logger;
		}

		public override int Order => 22;

		public override ActionPipelineBehavior PipelineBehavior => ActionPipelineBehavior.Break;

		public override async Task<ActionResult> ExecuteCoreAsync(Update update, Dictionary<string, object> metadata)
		{
			if (!metadata.TryGetValue(MetadataKeys.RepliedUsers, out var userIds))
			{
				_logger.LogWarning($"metadata key {MetadataKeys.RepliedUsers} not found");
				userIds = new HashSet<long>();
			}

			var botId = GetBotId(metadata);

			if (!_videoService.FrequencyCheck(botId))
			{
				return ActionResult.Continue;
			}

			var message = update.Message;
			var repliedUsersIds = (HashSet<long>)userIds;
			var randomVideo = _videoService.GetRandomVideo(botId);

			if (randomVideo == null)
			{
				return ActionResult.NotExecuted;
			}

			var botInstance = _botInstanceRepository.GetInstance(botId);

			await botInstance.Client.SendVideoAsync(
				message.Chat,
				randomVideo,
				message.MessageId);

			repliedUsersIds.Add(message.From.Id);

			return ActionResult.Succeed;
		}
	}
}
