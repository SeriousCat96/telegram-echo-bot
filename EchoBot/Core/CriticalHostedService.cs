using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.WebApp.Core
{
	public abstract class CriticalHostedService : IHostedService
	{
		private readonly IServiceScope _serviceScope;

		protected readonly ILogger<CriticalHostedService> Logger;
		protected readonly IServiceProvider ServiceProvider;

		protected abstract Task StartServiceAsync(CancellationToken cancellationToken);

		protected CriticalHostedService(
			ILogger<CriticalHostedService> logger, 
			IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScope = serviceScopeFactory.CreateScope();

			Logger = logger;
			ServiceProvider = _serviceScope.ServiceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				await StartServiceAsync(cancellationToken);
			}
			catch (Exception exception)
			{
				ExitCritical(exception);
			}
		}

		public virtual Task StopAsync(CancellationToken cancellationToken)
		{
			_serviceScope.Dispose();
			return Task.CompletedTask;
		}

		private void ExitCritical(Exception exception)
		{
			Console.WriteLine($"Critical application hosted service error in {GetType().Name}. Exception=[{exception}]");
			Logger.LogCritical(exception, $"Critical application hosted service error in {GetType().Name}");
			Environment.ExitCode = -1;
			ServiceProvider.GetRequiredService<IHostApplicationLifetime>().StopApplication();
		}
	}
}
