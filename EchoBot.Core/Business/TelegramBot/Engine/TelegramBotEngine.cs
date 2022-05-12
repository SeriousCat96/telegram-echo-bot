using EchoBot.Core.BackgroundJobs;
using EchoBot.Core.BackgroundJobs.SendMessage;
using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TelegramBot.Users;
using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EchoBot.Core.Business.TelegramBot.Engine
{
	public class TelegramBotEngine : ITelegramBotEngine
	{
		private const int TIMER_PERIOD_MILLISECONDS = 2000;
		private readonly ILogger<TelegramBotEngine> _logger;
		private readonly IEchoChatsService _echoChatsService;
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly BackgroundJobOptions _backgroundJobOptions;
		private readonly ICurrentUser _currentUser;
		private readonly Dictionary<string, IBotCommand> _commands;
		private readonly IEchoTelegramBotClient _botClient;
		private Timer _timer;
		private int? _offset;

		public TelegramBotEngine(
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService,
			ICurrentUser currentUser,
			IRecurringJobManager recurringJobManager,
			IOptions<BackgroundJobOptions> backgroundJobOptions,
			IEnumerable<IBotCommand> commands,
			ILogger<TelegramBotEngine> logger)
		{
			_botClient = botClient;
			_echoChatsService = chatsService;
			_recurringJobManager = recurringJobManager;
			_backgroundJobOptions = backgroundJobOptions.Value;
			_currentUser = currentUser;
			_logger = logger;
			_commands = new Dictionary<string, IBotCommand>();
			foreach (var command in commands)
			{
				var names = command.GetType().GetCustomAttributes<BotCommandAttribute>().Select(attr => attr.CommandName);
				foreach (var name in names)
				{
					_commands.Add(name, command);
				}
			}

			RunBackgroundScheduledJobs();
		}

		public void Dispose()
		{
			_timer.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken = default)
		{
			StartPolling(BotEngineTimerProc);
			return Task.CompletedTask;
		}

		private void StartPolling(Action<object> pollingAction)
		{
			_timer = new Timer(state => pollingAction(state), null, 0, TIMER_PERIOD_MILLISECONDS);
		}

		private async void BotEngineTimerProc(object state)
		{
			try
			{
				var updates = await _botClient.GetUpdatesAsync(_offset);

				foreach (var update in updates)
				{
					await ProcessUpdateAsync(update);
				}
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, exc.Message);
			}
		}

		private void RunBackgroundScheduledJobs()
		{
			if (_backgroundJobOptions.SendMessage.IsEnabled)
			{
				_recurringJobManager.AddOrUpdate<SendMessageBackgroundJob>(
					nameof(_backgroundJobOptions.SendMessage),
					job => job.ExecuteAsync(),
					_backgroundJobOptions.SendMessage.CronExpression);
			}
		}

		private static Message GetMessage(Update update)
		{
			return update.Message;
		}

		private async Task ProcessUpdateAsync(Update update)
		{
			var currentUser = _currentUser.User;

			if (currentUser == null)
			{
				return;
			}

			var uniqueUsersIds = new HashSet<long>();
			var message = GetMessage(update);

			_offset = update.Id + 1;

			if (message != null
				&& !string.IsNullOrWhiteSpace(message.Text)
				&& _commands.TryGetValue(message.Text, out var command))
			{
				await command.ExecuteCommandAsync(message);
			}
			else if (message != null
				&& uniqueUsersIds.Add(message.From.Id)
				&& message.From != null)
			{
				// Check reply to the bot message
				if (message.ReplyToMessage != null && message.ReplyToMessage.From?.Id == currentUser.Id)
				{
					var replyMessage = await _echoChatsService.GetRandomMessageAsync();
					await _botClient.SendMessageAsync(
						message.Chat,
						replyMessage.Text,
						replyToMessageId: message.MessageId,
						parseMode: ParseMode.Markdown);
				}
				// frequency check to subscribed users (to prevent spam)
				else if (_echoChatsService.FrequencyCheck())
				{
					var excludedUsers = _echoChatsService.GetExcludedUsers();
					string username = message.From.Username ?? message.From.Id.ToString();

					if (!excludedUsers.Any(name => name == username))
					{
						var replyMessage = await _echoChatsService.GetRandomMessageAsync();
						await _botClient.SendMessageAsync(
							message.Chat,
							replyMessage.Text,
							replyToMessageId: message.MessageId,
							parseMode: ParseMode.Markdown);
					}
				}
			}
		}
	}
}
