using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Commands;
using EchoBot.Telegram.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace EchoBot.Telegram.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTelegramBotEngineServices(this IServiceCollection services)
		{
			services.AddSingleton<ITelegramBotEngine, TelegramBotEngine>();
			services.AddSingleton<IActionsExecutor, ActionsExecutor>();
			services.AddSingleton<IBotCommandRepository, BotCommandRepository>();
			services.AddSingleton<ITelegramBotInstanceRepository, TelegramBotInstanceRepository>();

			return services;
		}
	}
}
