using System;

namespace EchoBot.Core.Business.TelegramBot
{
	public interface ITelegramBotEngine : IDisposable
	{
		void Start();
	}
}