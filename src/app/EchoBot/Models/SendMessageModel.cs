using Telegram.Bot.Types;

namespace EchoBot.WebApp.Models
{
	public class SendMessageModel
	{
		public long ChatId { get; set; }
		public string Message { get; set; }
		public int? ReplyToMessageId { get; set; }
	}
}
