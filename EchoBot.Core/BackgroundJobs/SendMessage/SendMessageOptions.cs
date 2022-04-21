namespace EchoBot.Core.BackgroundJobs.SendMessage
{
	public class SendMessageOptions
	{
		public string CronExpression { get; set; }
		public long ChatId { get; set; }
	}
}
