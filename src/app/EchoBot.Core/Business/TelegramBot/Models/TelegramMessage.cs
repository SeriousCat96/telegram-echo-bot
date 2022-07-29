namespace EchoBot.Core.Business.TelegramBot.Models
{
	public class TelegramMessage
	{
		public string Text { get; set; }

		public static implicit operator TelegramMessage(string text) => new TelegramMessage
		{
			Text = text
		};
	}
}
