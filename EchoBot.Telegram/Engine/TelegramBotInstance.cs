﻿using EchoBot.Telegram.Actions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Engine
{
	public class TelegramBotInstance : ITelegramBotInstance
	{
		private const int TIMER_PERIOD_MILLISECONDS = 2000;
		private readonly ILogger<TelegramBotInstance> _logger;
		private readonly IActionsExecutor _actionsExecutor;
		private readonly SemaphoreSlim _semaphore;
		private Timer _timer;
		private int? _offset;

		public IEchoTelegramBotClient Client { get; set; }
		public int BotId { get; }
		public User User => Client.User;

		public TelegramBotInstance(
			int botId,
			IEchoTelegramBotClient client,
			IActionsExecutor actionsExecutor,
			ILogger<TelegramBotInstance> logger)
		{
			BotId = botId;
			Client = client;
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

				var updates = await Client.GetUpdatesAsync(_offset);
				if (updates.Length == 0)
				{
					return;
				}

				var metadata = new Dictionary<string, object>
				{
					[MetadataKeys.RepliedUsers] = new HashSet<long>(),
					[MetadataKeys.User] = User,
					[MetadataKeys.BotId] = BotId
				};

				foreach (var update in updates)
				{
					_offset = update.Id + 1;

					if (update.Message != null)
					{
						_logger.LogDebug("Incoming message ({0}): \"{1}\" from {2}", User.Username, update.Message.Text, update.Message.Chat.Id);
					}

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