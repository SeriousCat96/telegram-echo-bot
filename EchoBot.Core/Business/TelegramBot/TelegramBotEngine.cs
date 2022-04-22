using EchoBot.Core.BackgroundJobs;
using EchoBot.Core.BackgroundJobs.SendMessage;
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
using Telegram.Bot.Types;

namespace EchoBot.Core.Business.TelegramBot
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

		public void Start()
		{
			_timer = new Timer(BotEngineTimerProc, null, 0, TIMER_PERIOD_MILLISECONDS);
		}

		private async void BotEngineTimerProc(object state)
		{
			try
			{
				var currentUser = _currentUser.User;
				if (currentUser == null)
				{
					return;
				}

				var uniqueUsersIds = new HashSet<long>();
				var updates = await _botClient.GetUpdatesAsync(_offset);

				foreach (var update in updates)
				{
					_offset = update.Id + 1;

					var message = GetMessage(update);

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
							var replyMsgText = _echoChatsService.GetRandomMessage();
							await _botClient.SendMessageAsync(message.Chat, replyMsgText, replyToMessageId: message.MessageId);
						}
						// frequency check to subscribed users (to prevent spam)
						else if (_echoChatsService.FrequencyCheck())
						{
							var userIds = _echoChatsService.GetUsers();
							string userId = message.From.Username ?? message.From.Id.ToString();

							if (userIds.Any(id => id == userId))
							{
								var replyMsgText = _echoChatsService.GetRandomMessage();
								await _botClient.SendMessageAsync(message.Chat, replyMsgText, replyToMessageId: message.MessageId);
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, exc.Message);
			}
		}

		private static Message GetMessage(Update update)
		{
			return update.Message;
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
	}
}
