using EchoBot.Core.Business.TelegramBot.Engine;
using EchoBot.WebApp.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.WebApp.HostedServices
{
	public class TelegramBotHostedService : CriticalHostedService
	{
		private readonly ITelegramBotEngine _telegramBotEngine;

		public TelegramBotHostedService(
			ITelegramBotEngine telegramBotEngine,
			ILogger<TelegramBotHostedService> logger, 
			IServiceScopeFactory serviceScopeFactory) 
			: base(logger, serviceScopeFactory)
		{
			_telegramBotEngine = telegramBotEngine;
		}

		protected override async Task StartServiceAsync(CancellationToken cancellationToken)
		{
			await _telegramBotEngine.StartAsync();
		}
	}
}
