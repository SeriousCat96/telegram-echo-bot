using AutoMapper;
using EchoBot.Core.Options;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Telegram.Engine
{
	public class TelegramBotEngine : ITelegramBotEngine
	{
		private readonly ILogger<TelegramBotEngine> _logger;
		private readonly ITelegramBotInstanceRepository _telegramBotInstanceRepository;

		public TelegramBotEngine(
			IActionsExecutor actionsExecutor,
			ILoggerFactory loggerFactory,
			IOptions<BotsOptions> botsOptions,
			IHubContext<MessagesHub> messageHubContext,
			IMapper mapper,
			ITelegramBotInstanceRepository telegramBotInstanceRepository,
			ILogger<TelegramBotEngine> logger)
		{
			_logger = logger;
			_telegramBotInstanceRepository = telegramBotInstanceRepository;

			var clientLogger = loggerFactory.CreateLogger<EchoTelegramBotClient>();
			var instanceLogger = loggerFactory.CreateLogger<TelegramBotInstance>();

			var botInstances = botsOptions
				.Value
				.Bots
				.Select(opt => new TelegramBotInstance(
					opt.Id,
					messageHubContext,
					mapper,
					new EchoTelegramBotClient(messageHubContext, mapper, clientLogger, opt),
					actionsExecutor,
					instanceLogger
				));

			foreach (var instance in botInstances)
			{
				telegramBotInstanceRepository.AddInstance(instance);
			}
		}

		public void Dispose()
		{
			_telegramBotInstanceRepository.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken = default)
		{
			foreach (var instance in _telegramBotInstanceRepository)
			{
				try
				{
					instance.StartAsync();
				}
				catch (Exception exc)
				{
					_logger.LogError(exc, "Error occured when starting bot");
				}
			}

			return Task.CompletedTask;
		}
	}
}
