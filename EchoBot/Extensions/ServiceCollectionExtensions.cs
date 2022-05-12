using EchoBot.Core.BackgroundJobs;
using EchoBot.Core.BackgroundJobs.SendMessage;
using EchoBot.Core.Business.ChatsService;
using EchoBot.Core.Business.TemplateParser;
using EchoBot.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EchoBot.WebApp.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTelegramBotOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<TelegramBotOptions>(configuration.GetSection("BotOptions"));
			services.Configure<EchoChatOptions>(configuration.GetSection("ChatOptions"));
			services.Configure<TemplateOptions>(configuration.GetSection("Templates"));
			services.Configure<BackgroundJobOptions>(configuration.GetSection("BackgroundJobs"));
			services.Configure<SendMessageOptions>(configuration.GetSection("BackgroundJobs:SendMessage"));

			return services;
		}
	}
}
