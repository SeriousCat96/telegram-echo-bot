using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.TelegramBot.Engine
{
	public interface ITelegramBotEngine : IDisposable
	{
		Task StartAsync(CancellationToken cancellationToken = default);
	}
}