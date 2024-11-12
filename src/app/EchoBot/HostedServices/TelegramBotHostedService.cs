using EchoBot.Core.Options;
using EchoBot.Telegram.Engine;
using EchoBot.WebApp.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.WebApp.HostedServices
{
	public class TelegramBotHostedService : CriticalHostedService
	{
		private readonly ITelegramBotEngine _telegramBotEngine;
		private readonly ITelegramBotInstanceRepository _telegramBotInstanseRepository;
		private readonly IOptions<BotsOptions> _botOptions;
		private readonly IConfiguration _configuration;

		public TelegramBotHostedService(
			ITelegramBotEngine telegramBotEngine,
			ILogger<TelegramBotHostedService> logger, 
			IServiceScopeFactory serviceScopeFactory,
			ITelegramBotInstanceRepository telegramBotInstanseRepository,
			IOptions<BotsOptions> botOptions,
			IConfiguration configuration) 
			: base(logger, serviceScopeFactory)
		{
			_telegramBotEngine = telegramBotEngine;
			_telegramBotInstanseRepository = telegramBotInstanseRepository;
			_botOptions = botOptions;
			_configuration = configuration;
		}

		protected override async Task StartServiceAsync(CancellationToken cancellationToken)
		{
			bool useWebhook = _configuration.GetValue<bool?>("UseWebhook") ?? false;

			if (useWebhook)
			{
				var url = $"{_botOptions.Value.ClientUrl}api/bot/webhook";
				var tasks = _telegramBotInstanseRepository
					.Select(bot => bot.Client.SetWebhook(url + "/" + bot.BotId, cancellationToken: cancellationToken));

				await Task.WhenAll(tasks);
			}
			else
			{
				var tasks = _telegramBotInstanseRepository.Select(bot => bot.Client.DeleteWebhook(cancellationToken: cancellationToken));

				await Task.WhenAll(tasks);

				await _telegramBotEngine.StartAsync();
			}
		}

		protected override Task StopServiceAsync(CancellationToken cancellationToken)
		{
			_telegramBotEngine.Dispose();

			return Task.CompletedTask;
		}
	}
}
