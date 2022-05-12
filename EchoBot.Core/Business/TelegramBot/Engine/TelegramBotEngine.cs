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
		private readonly IBotCommandRepository _commandRepository;
		private readonly IEchoChatsService _echoChatsService;
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly BackgroundJobOptions _backgroundJobOptions;
		private readonly ICurrentUser _currentUser;
		private readonly IEchoTelegramBotClient _botClient;
		private Timer _timer;
		private int? _offset;

		public TelegramBotEngine(
			IEchoTelegramBotClient botClient,
			IEchoChatsService chatsService,
			ICurrentUser currentUser,
			IRecurringJobManager recurringJobManager,
			IOptions<BackgroundJobOptions> backgroundJobOptions,
			IBotCommandRepository commandRepository,
			ILogger<TelegramBotEngine> logger)
		{
			_botClient = botClient;
			_echoChatsService = chatsService;
			_recurringJobManager = recurringJobManager;
			_backgroundJobOptions = backgroundJobOptions.Value;
			_currentUser = currentUser;
			_logger = logger;
			_commandRepository = commandRepository;

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

			if (message == null)
			{
				return;
			}

			var command = _commandRepository.GetCommandByName(message.Text);
			if (command != null)
			{
				await command.ExecuteCommandAsync(message);
				return;
			}

			if (uniqueUsersIds.Add(message.From.Id) && message.From != null)
			{
				// Check reply to the bot message
				if (message.ReplyToMessage != null && message.ReplyToMessage.From?.Id == currentUser.Id)
				{
					await SendReplyToMessageAsync(message);
				}
				// frequency check to subscribed users (to prevent spam)
				else if (_echoChatsService.FrequencyCheck())
				{
					var excludedUsers = _echoChatsService.GetExcludedUsers();
					string username = message.From.Username ?? message.From.Id.ToString();

					if (!excludedUsers.Any(name => name == username))
					{
						await SendReplyToMessageAsync(message);
					}
				}
			}
		}

		private async Task SendReplyToMessageAsync(Message receivedMessage)
		{
			var replyMessage = await _echoChatsService.GetRandomMessageAsync();
			await _botClient.SendMessageAsync(
				receivedMessage.Chat,
				replyMessage.Text,
				replyToMessageId: receivedMessage.MessageId,
				parseMode: ParseMode.Markdown);
		}
	}
}
