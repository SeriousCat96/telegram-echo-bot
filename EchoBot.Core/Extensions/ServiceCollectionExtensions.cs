using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TemplateParser;
using EchoBot.Telegram.Actions;
using EchoBot.Telegram.Actions.Filters;
using EchoBot.Telegram.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace EchoBot.Core.Business.TelegramBot.Commands
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTelegramBotCommands(this IServiceCollection services)
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

		public static IServiceCollection AddTelegramBotActions(this IServiceCollection services)
		{
			var commandTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(IAction).IsAssignableFrom(type) && !type.IsAbstract);

			var filterTypes = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(IActionFilter).IsAssignableFrom(type) && !type.IsAbstract);

			foreach (var type in commandTypes)
			{
				services.AddSingleton(typeof(IAction), type);
			}

			foreach (var type in filterTypes)
			{
				services.AddSingleton(type);
			}

			return services;
		}

		public static IServiceCollection AddTelegramBotServices(this IServiceCollection services)
		{
			services.AddSingleton<IEchoChatsService, EchoChatsService>();
			services.AddSingleton<ITemplateMessageParser, TemplateMessageParser>();

			return services;
		}
	}
}
