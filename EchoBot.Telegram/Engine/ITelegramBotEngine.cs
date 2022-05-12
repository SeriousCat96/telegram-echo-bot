using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Telegram.Engine
{
	public interface ITelegramBotEngine : IDisposable
	{
		Task StartAsync(CancellationToken cancellationToken = default);
	}
}