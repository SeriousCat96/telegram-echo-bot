using EchoBot.Telegram.Users;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EchoBot.Telegram.Commands
{
	public class BotCommandRepository : IBotCommandRepository
	{
		private static readonly Regex _commandExpr = new Regex(@"^(?<commandName>/[a-zA-Z0-9_]+)+(?<botName>@[a-zA-Z0-9_]+)?", RegexOptions.Compiled);
		
		private readonly ICurrentUser _currentUser;
		private readonly Dictionary<string, IBotCommand> _commands;

		public BotCommandRepository(IEnumerable<IBotCommand> commands, ICurrentUser currentUser)
		{
			_currentUser = currentUser;
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
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			var commandName = GetCommandName(name);
			if (string.IsNullOrWhiteSpace(commandName))
			{
				return null;
			}

			_commands.TryGetValue(commandName, out var command);
			return command;
		}

		private string GetCommandName(string input)
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
			if (commandNameGroupMatch.Success && $"@{_currentUser.User.Username}" != botNameGroupMatch.Value)
			{
				return null;
			}

			return commandNameGroupMatch.Value;
		}
	}
}
