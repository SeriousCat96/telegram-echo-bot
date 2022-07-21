namespace EchoBot.Telegram.Options
{
	public class SendMessageOptions
	{
		public bool IsEnabled { get; set; }
		public string CronExpression { get; set; }
		public long ChatId { get; set; }
	}
}
