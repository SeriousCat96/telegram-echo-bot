using System;

namespace EchoBot.Telegram.Commands
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BotCommandAttribute : Attribute
	{
		public BotCommandAttribute(string commandName)
		{
			CommandName = commandName;
		}

		public string CommandName { get; }
	}
}
