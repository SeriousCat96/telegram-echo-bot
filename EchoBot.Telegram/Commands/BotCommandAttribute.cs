using System;

namespace EchoBot.Telegram.Commands
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BotCommandAttribute : Attribute
	{
		public BotCommandAttribute(string commandName, string description)
		{
			CommandName = commandName;
			Description = description;
		}

		public string CommandName { get; }

		public string Description { get; }
	}
}
