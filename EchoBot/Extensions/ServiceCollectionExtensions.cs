using EchoBot.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EchoBot.WebApp.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTelegramBotOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<BotsOptions>(configuration.GetSection("BotsOptions"));

			return services;
		}
	}
}
