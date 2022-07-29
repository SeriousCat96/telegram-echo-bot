using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EchoBot.Telegram.Engine
{
	public class TelegramBotInstanceRepository : ITelegramBotInstanceRepository
	{
		private readonly ILogger<TelegramBotInstanceRepository> _logger;
		private readonly Dictionary<int, ITelegramBotInstance> _botInstances;

		public TelegramBotInstanceRepository(
			ILogger<TelegramBotInstanceRepository> logger)
		{
			_logger = logger;
			_botInstances = new Dictionary<int, ITelegramBotInstance>();
		}

		public ITelegramBotInstance GetInstance(int id)
		{
			_botInstances.TryGetValue(id, out var instance);
			return instance;
		}

		public void Dispose()
		{
			foreach (var instance in this)
			{
				try
				{
					instance.Dispose();
				}
				catch (Exception exc)
				{
					_logger.LogError(exc, "Error occured when disposing bot");
				}
			}
		}

		public IEnumerator<ITelegramBotInstance> GetEnumerator() => _botInstances.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _botInstances.Values.GetEnumerator();

		public void AddInstance(ITelegramBotInstance botInstance)
		{
			_botInstances.Add(botInstance.BotId, botInstance);
		}
	}
}
