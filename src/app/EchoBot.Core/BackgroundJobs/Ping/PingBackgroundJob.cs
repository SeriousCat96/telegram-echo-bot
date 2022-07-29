using EchoBot.Integration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Core.BackgroundJobs.SendMessage
{
	public class PingBackgroundJob
	{
		private readonly ILogger<PingBackgroundJob> _logger;
		private readonly IEchoBotHttpClient _botHttpClient;

		public PingBackgroundJob(
			ILogger<PingBackgroundJob> logger,
			IEchoBotHttpClient botHttpClient)
		{
			_logger = logger;
			_botHttpClient = botHttpClient;
		}

		public async Task ExecuteAsync(int botId, CancellationToken cancellationToken = default)
		{
			var result = await _botHttpClient.TestApiAsync(botId);
			_logger.LogDebug($"Test bot id={botId} result: {result}");
		}
	}
}
