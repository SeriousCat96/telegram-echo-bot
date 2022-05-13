using EchoBot.Core.Business.TelegramBot.Commands;
using EchoBot.Telegram.Extensions;
using EchoBot.WebApp.Extensions;
using EchoBot.WebApp.HostedServices;
using Hangfire;
using Hangfire.MemoryStorage;
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
			services.AddMvc();
			services.AddControllers();
			services.AddSwaggerDocument(document =>
			{
				document.Title = Assembly.GetEntryAssembly().GetName().Name;
				document.DocumentName = "v1";
				document.IgnoreObsoleteProperties = true;
			});

			services.AddHangfireServer();
			services.AddHangfire(config =>
				config.UseMemoryStorage()
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
			);

			services.AddTelegramBotEngineServices();
			services.AddTelegramBotServices();
			services.AddTelegramBotCommands();
			services.AddTelegramBotActions();
			services.AddTelegramBotOptions(Configuration);

			services.AddHostedService<TelegramBotHostedService>();
			services.AddHostedService<HangfireHostedService>();
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
			app.UseHangfireDashboard();

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
