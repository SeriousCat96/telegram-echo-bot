using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EchoBot.Telegram.Commands
{
	public class BotCommandRepository : IBotCommandRepository
	{
		private static readonly Regex _commandExpr = new Regex(@"^(?<commandName>/[a-zA-Z0-9_]+)+(?<botName>@[a-zA-Z0-9_]+)?", RegexOptions.Compiled);
		
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

		public IBotCommand GetCommandByName(string name, string username)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			var commandName = GetCommandName(name, username);
			if (string.IsNullOrWhiteSpace(commandName))
			{
				return null;
			}

			_commands.TryGetValue(commandName, out var command);
			return command;
		}

		private string GetCommandName(string input, string username)
		{
			var match = _commandExpr.Match(input);
			if (!match.Success)
			{
				return null;
			}

			var commandNameGroupMatch = match.Groups["commandName"];
			if (!commandNameGroupMatch.Success)
			{
				return null;
			}

			var botNameGroupMatch = match.Groups["botName"];
			if (!string.IsNullOrWhiteSpace(botNameGroupMatch.Value) && username != botNameGroupMatch.Value)
			{
				return null;
			}

			return commandNameGroupMatch.Value;
		}
	}
}
