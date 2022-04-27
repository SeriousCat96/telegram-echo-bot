using System;

namespace EchoBot.Core.Business.TelegramBot.Engine
{
	public interface ITelegramBotEngine : IDisposable
	{
		void Start();
	}
}