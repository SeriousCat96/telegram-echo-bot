using System;

namespace EchoBot.Telegram.Options
{
	public class EchoChatOptions
	{
		public string[] Users { get; set; } = Array.Empty<string>();
		public string[] ExcludedUsers { get; set; } = Array.Empty<string>();
		public string[] Messages { get; set; }
		public VideoOptions Videos { get; set; }
		public string ChatId { get; set; }
		public int ReplyFrequency { get; set; }
	}
}
