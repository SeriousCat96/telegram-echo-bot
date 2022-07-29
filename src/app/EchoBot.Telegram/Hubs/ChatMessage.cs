using System;

namespace EchoBot.Telegram.Hubs
{
	public class ChatMessage
	{
		public long Id { get; set; }
		public ChatUser FromUser { get; set; }
		public Chat Chat { get; set; }
		public Bot Bot { get; set; }
		public DateTime Date { get; set; }
		public string Text { get; set; }
		public bool FromBot { get; set; }
	}
}
