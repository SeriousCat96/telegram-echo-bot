using System;
using System.Collections.Generic;

namespace EchoBot.Telegram.Engine
{
	public interface ITelegramBotInstanceRepository : IEnumerable<ITelegramBotInstance>, IDisposable
	{
		void AddInstance(ITelegramBotInstance botInstance);

		ITelegramBotInstance GetInstance(int id);
	}
}
