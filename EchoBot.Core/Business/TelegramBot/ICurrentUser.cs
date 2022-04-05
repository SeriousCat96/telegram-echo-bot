using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot
{
	public interface ICurrentUser
	{
		User User { get; }
	}
}
