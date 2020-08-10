using HealthCheckHostServer;
using HealthCheckHostServer.Configuration;
using HealthCheckHostServer.Core.Data;
using HealthCheckHostServer.Core.HostedService;
using HealthCheckHostServer.Core.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace HealthChecksService.Extensions
{
    public static  class ServiceCollectionExtensions
    {
        public static HealthChecksUIBuilder AddHealthChecksTaskService(this IServiceCollection services,
            Action<Settings> setupSettings = null)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.PostConfigure<Settings>(options =>
            {
                options.EvaluationTimeInSeconds = int.Parse(config["EvaluationTimeInSeconds"]);
            });

            // server
            services.AddSingleton<IHostedService, HealthCheckCollectorHostedService>();

            // 實作
            services.AddScoped<IHealthCheckReportCollector, HealthCheckReportCollector>();
            services.AddScoped<IHealthCheckFailureNotifier, WebHookFailureNotifier>();

            services.AddApiEndpointHttpClient();
            services.AddWebhooksEndpointHttpClient();

            // sqlserver
            services.AddDbContext<HealthChecksDb>(db =>
            {
                var connectionString = config["APIdevelopers"];

                db.UseSqlServer(connectionString);
            });

            return new HealthChecksUIBuilder(services);
        }

        public static IServiceCollection AddApiEndpointHttpClient(this IServiceCollection services)
        {
            return services.AddHttpClient(Keys.HEALTH_CHECK_HTTP_CLIENT_NAME, (sp, client) =>
            {
                var settings = sp.GetService<IOptions<Settings>>();
                settings.Value.ApiEndpointHttpClientConfig?.Invoke(sp, client);
            }).ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var settings = sp.GetService<IOptions<Settings>>();
                return settings.Value.ApiEndpointHttpHandler?.Invoke(sp) ?? new HttpClientHandler();
            }).SetHandlerLifetime(TimeSpan.FromSeconds(2))
            .Services;
        }

        public static IServiceCollection AddWebhooksEndpointHttpClient(this IServiceCollection services)
        {
            // health-checks-webhooks
           return services.AddHttpClient(Keys.HEALTH_CHECK_WEBHOOK_HTTP_CLIENT_NAME, (sp, client) =>
            {
                var settings = sp.GetService<IOptions<Settings>>();
                settings.Value.WebHooksEndpointHttpClientConfig?.Invoke(sp, client);
            })
           .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var settings = sp.GetService<IOptions<Settings>>();
                return settings.Value.WebHooksEndpointHttpHandler?.Invoke(sp) ?? new HttpClientHandler();
            }).SetHandlerLifetime(TimeSpan.FromSeconds(2))
           .Services;
        }

    }
}
