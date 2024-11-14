using EchoBot.Core.Business.ChatsService;
using EchoBot.Telegram.Actions;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageVideoReplyFrequencyActionFilter : MessageNotEmptyActionFilter
	{
		private readonly IVideoService _videoService;

		public override int Order => 40;

		public MessageVideoReplyFrequencyActionFilter(IVideoService videoService)
		{
			_videoService = videoService;
		}

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata)
		{
			metadata.TryGetValue(MetadataKeys.BotId, out var botId);
			return base.IsAllowed(update, metadata) && update.Message.Chat.Type == ChatType.Private
				|| _videoService.FrequencyCheck((int)botId);
		}
	}
}
