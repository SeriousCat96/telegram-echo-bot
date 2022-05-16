using EchoBot.Telegram.Actions.Filters;
using EchoBot.Telegram.Users;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Actions.Filters
{
	public class MessageToBotActionFilter : MessageNotEmptyActionFilter
	{
		private readonly ICurrentUser _currentUser;

		public override int Order => 20;

		public MessageToBotActionFilter(ICurrentUser currentUser)
		{
			_currentUser = currentUser;
		}

		public override bool IsAllowed(Update update, Dictionary<string, object> metadata) 
			=> base.IsAllowed(update, metadata)
			&& _currentUser.User?.Id == update.Message.ReplyToMessage?.From?.Id;
	}
}
