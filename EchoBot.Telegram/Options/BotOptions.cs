using EchoBot.Telegram.Options;

namespace EchoBot.Core.Options
{
	public class BotOptions
	{
		public int Id { get; set; }

		public string Token { get; set; }

		public EchoChatOptions ChatOptions { get; set; }

		public TemplateOptions Templates { get; set; }

		public BackgroundJobOptions BackgroundJobs { get; set; }
	}
}
