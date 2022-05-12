using Telegram.Bot.Types;

namespace EchoBot.Telegram.Users
{
	public interface ICurrentUser
	{
		User User { get; }
	}
}
