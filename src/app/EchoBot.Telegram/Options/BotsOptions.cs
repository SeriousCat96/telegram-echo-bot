using System;

namespace EchoBot.Core.Options
{
	public class BotsOptions
	{
		public Uri ClientUrl { get; set; }
		public BotOptions[] Bots { get; set; }
	}
}
