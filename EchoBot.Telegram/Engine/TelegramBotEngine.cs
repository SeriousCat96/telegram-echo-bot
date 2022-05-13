using EchoBot.Telegram.Actions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Engine
{
	public class TelegramBotEngine : ITelegramBotEngine
	{
		private const int TIMER_PERIOD_MILLISECONDS = 2000;
		private readonly ILogger<TelegramBotEngine> _logger;
		private readonly IEchoTelegramBotClient _botClient;
		private readonly IActionsExecutor _actionsExecutor;
		private readonly SemaphoreSlim _semaphore;
		private Timer _timer;
		private int? _offset;

		public TelegramBotEngine(
			IEchoTelegramBotClient botClient,
			IActionsExecutor actionsExecutor,
			ILogger<TelegramBotEngine> logger)
		{
			_botClient = botClient;
			_actionsExecutor = actionsExecutor;
			_logger = logger;
			_semaphore = new SemaphoreSlim(1, 1);
		}

		public void Dispose()
		{
			_timer.Dispose();
			_semaphore.Dispose();
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
				await _semaphore.WaitAsync();

				var updates = await _botClient.GetUpdatesAsync(_offset);
				if (updates.Length == 0)
				{
					return;
				}

				var metadata = new Dictionary<string, object>
				{
					[MetadataKeys.RepliedUsers] = new HashSet<long>()
				};

				foreach (var update in updates)
				{
					_offset = update.Id + 1;
					await _actionsExecutor.ExecuteAsync(update, metadata);
				}
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, exc.Message);
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}
