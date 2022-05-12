using EchoBot.Core.BackgroundJobs;
using EchoBot.Core.BackgroundJobs.SendMessage;
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
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly BackgroundJobOptions _backgroundJobOptions;

		public HangfireHostedService(
			IRecurringJobManager recurringJobManager,
			IOptions<BackgroundJobOptions> backgroundJobOptions,
			ILogger<HangfireHostedService> logger,
			IServiceScopeFactory serviceScopeFactory)
			: base(logger, serviceScopeFactory)
		{
			_recurringJobManager = recurringJobManager;
			_backgroundJobOptions = backgroundJobOptions.Value;
		}

		protected override Task StartServiceAsync(CancellationToken cancellationToken)
		{
			if (_backgroundJobOptions.SendMessage.IsEnabled)
			{
				_recurringJobManager.AddOrUpdate<SendMessageBackgroundJob>(
					nameof(_backgroundJobOptions.SendMessage),
					job => job.ExecuteAsync(cancellationToken),
					_backgroundJobOptions.SendMessage.CronExpression);
			}

			return Task.CompletedTask;
		}

		protected override Task StopServiceAsync(CancellationToken cancellationToken)
		{
			_recurringJobManager.RemoveIfExists(nameof(_backgroundJobOptions.SendMessage));

			return Task.CompletedTask;
		}
	}
}
