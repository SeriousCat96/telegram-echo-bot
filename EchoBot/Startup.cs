using EchoBot.Core;
using EchoBot.Core.Business;
using EchoBot.Core.Business.TelegramBot;
using EchoBot.Core.Business.TelegramBot.Commands;
using EchoBot.Telegram;
using EchoBot.Telegram.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace EchoBot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<TelegramBotOptions>(Configuration.GetSection("BotOptions"));
			services.Configure<EchoChatOptions>(Configuration.GetSection("ChatOptions"));
			services.Configure<CommandsOptions>(Configuration.GetSection("Commands"));
			services.Configure<TemplateOptions>(Configuration.GetSection("Templates"));

			services.AddMvc();
			services.AddControllers();
			services.AddSwaggerDocument(document =>
			{
				document.Title = Assembly.GetEntryAssembly().GetName().Name;
				document.DocumentName = "v1";
				document.IgnoreObsoleteProperties = true;
			});

			services.AddSingleton<ITelegramBotEngine, TelegramBotEngine>();
			services.AddSingleton<IEchoTelegramBotClient, EchoTelegramBotClient>();
			services.AddSingleton<IEchoChatsService, EchoChatsService>();
			services.AddSingleton<ICurrentUser, CurrentUser>();
			services.AddSingleton<ITemplateMessageParser, TemplateMessageParser>();
			services.AddSingleton<IBotCommand, StartBotCommand>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseOpenApi();
			app.UseSwaggerUi3();
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "api",
					pattern: "api/{controller}/{id?}"
				);
			});
		}
	}
}
