using EchoBot.Telegram.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace EchoBot.Core.Business.TelegramBot.Commands
{
	public static class BotCommandsDependencyInjectionExtensions
	{
		public static IServiceCollection RegisterBotCommands(this IServiceCollection services)
		{
			var commandTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(IBotCommand).IsAssignableFrom(type));

			foreach (var type in commandTypes)
			{
				services.AddSingleton(typeof(IBotCommand), type);
			}

			return services;
		}
	}
}
