using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Commands;
using EchoBot.Telegram.Engine;
using EchoBot.Telegram.Users;
using Microsoft.Extensions.DependencyInjection;

namespace EchoBot.Telegram.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTelegramBotEngineServices(this IServiceCollection services)
		{
			services.AddSingleton<ITelegramBotEngine, TelegramBotEngine>();
			services.AddSingleton<IEchoTelegramBotClient, EchoTelegramBotClient>();
			services.AddSingleton<IActionsExecutor, ActionsExecutor>();
			services.AddSingleton<IBotCommandRepository, BotCommandRepository>();
			services.AddSingleton<ICurrentUser, CurrentUser>();

			return services;
		}
	}
}
