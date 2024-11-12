using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Engine
{
	public interface ITelegramBotInstance : IDisposable
	{
		int BotId { get; }

		User User { get; }

		IEchoTelegramBotClient Client { get; set; }

		Task StartAsync(CancellationToken cancellationToken = default);

		Task ProcessUpdates(IReadOnlyCollection<Update> updates);
	}
}