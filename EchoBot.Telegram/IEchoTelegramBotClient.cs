using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram
{
	public interface IEchoTelegramBotClient
	{
		Task<bool> TestApiAsync();

		Task<Message> SendMessageAsync(ChatId chatId, string message, int? replyToMessageId = default);
	}
}
