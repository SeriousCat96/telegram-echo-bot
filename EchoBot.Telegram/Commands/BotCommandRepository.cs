using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EchoBot.Telegram.Commands
{
	public class BotCommandRepository : IBotCommandRepository
	{
		private readonly Dictionary<string, IBotCommand> _commands;

		public BotCommandRepository(IEnumerable<IBotCommand> commands)
		{
			_commands = new Dictionary<string, IBotCommand>();
			foreach (var command in commands)
			{
				var names = command.GetType().GetCustomAttributes<BotCommandAttribute>().Select(attr => attr.CommandName);
				foreach (var name in names)
				{
					_commands.Add(name, command);
				}
			}
		}

		public IBotCommand GetCommandByName(string name)
		{
			_commands.TryGetValue(name, out var command);
			return command;
		}
	}
}
