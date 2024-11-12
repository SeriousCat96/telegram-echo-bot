using AutoMapper;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Hubs;
using Microsoft.AspNetCore.SignalR;
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
		private readonly IHubContext<MessagesHub> _messageHubContext;
		private readonly IMapper _mapper;
		private readonly IActionsExecutor _actionsExecutor;
		private readonly SemaphoreSlim _semaphore;
		private Timer _timer;
		private int? _offset;

		public IEchoTelegramBotClient Client { get; set; }


		public int BotId { get; }
		public User User => Client.User;

		public TelegramBotInstance(
			int botId,
			IHubContext<MessagesHub> messageHubContext,
			IMapper mapper,
			IEchoTelegramBotClient client,
			IActionsExecutor actionsExecutor,
			ILogger<TelegramBotInstance> logger)
		{
			BotId = botId;
			Client = client;
			_messageHubContext = messageHubContext;
			_mapper = mapper;
			_actionsExecutor = actionsExecutor;
			_logger = logger;
			_semaphore = new SemaphoreSlim(1, 1);
		}

		public void Dispose()
		{
			_timer?.Dispose();
			_semaphore?.Dispose();
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

				await ProcessUpdates(updates);
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

		public async Task ProcessUpdates(IReadOnlyCollection<Update> updates)
		{
			try
			{
				if (updates.Count == 0)
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
						_logger.LogDebug("Incoming message ({0}): \"{1}\" from {2} (user: {3})", User.Username, update.Message.Text, update.Message.Chat.Id, update.Message.From);
						
						var chatMessage = _mapper.Map<ChatMessage>(update.Message);
						var bot = _mapper.Map<Bot>(User);

						chatMessage.Bot = bot;

						await _messageHubContext.Clients.All.SendAsync("Message", chatMessage);
					}

					await _actionsExecutor.ExecuteAsync(update, metadata);
				}
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, exc.Message);
			}
		}
	}
}
