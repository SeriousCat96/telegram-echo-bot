using EchoBot.Core.BackgroundJobs.SendMessage;
using EchoBot.Core.Options;
using EchoBot.Telegram.Engine;
using EchoBot.WebApp.Core;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.WebApp.HostedServices
{
	public class HangfireHostedService : CriticalHostedService
	{
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly BotOptions[] _botsOptions;

		public HangfireHostedService(
			ITelegramBotInstanceRepository botInstanceRepository,
			IRecurringJobManager recurringJobManager,
			IOptions<BotsOptions> backgroundJobOptions,
			ILogger<HangfireHostedService> logger,
			IServiceScopeFactory serviceScopeFactory)
			: base(logger, serviceScopeFactory)
		{
			_botInstanceRepository = botInstanceRepository;
			_recurringJobManager = recurringJobManager;
			_botsOptions = backgroundJobOptions.Value.Bots;
		}

		protected override Task StartServiceAsync(CancellationToken cancellationToken)
		{
			foreach (var botOptions in _botsOptions)
			{
				if (botOptions.BackgroundJobs.SendMessage.IsEnabled)
				{
					_recurringJobManager.AddOrUpdate<SendMessageBackgroundJob>(
						$"{nameof(botOptions.BackgroundJobs.SendMessage)}{botOptions.Id}",
						job => job.ExecuteAsync(botOptions.Id, cancellationToken),
						botOptions.BackgroundJobs.SendMessage.CronExpression);
				}
				if (botOptions.BackgroundJobs.Ping.IsEnabled)
				{
					_recurringJobManager.AddOrUpdate<PingBackgroundJob>(
						$"{nameof(botOptions.BackgroundJobs.Ping)}{botOptions.Id}",
						job => job.ExecuteAsync(botOptions.Id, cancellationToken),
						botOptions.BackgroundJobs.Ping.CronExpression);
				}
			}


			return Task.CompletedTask;
		}

		protected override Task StopServiceAsync(CancellationToken cancellationToken)
		{
			foreach (var botOptions in _botsOptions)
			{
				_recurringJobManager.RemoveIfExists($"{nameof(botOptions.BackgroundJobs.SendMessage)}{botOptions.Id}");
				_recurringJobManager.RemoveIfExists($"{nameof(botOptions.BackgroundJobs.Ping)}{botOptions.Id}");
			}
			

			return Task.CompletedTask;
		}
	}
}
