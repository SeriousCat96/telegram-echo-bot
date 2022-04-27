using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot.Users
{
	public interface ICurrentUser
	{
		User User { get; }
	}
}
