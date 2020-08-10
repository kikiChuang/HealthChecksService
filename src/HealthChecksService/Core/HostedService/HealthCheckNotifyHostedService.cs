using HealthCheckHostServer.Configuration;
using HealthCheckHostServer.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckHostServer.Core.HostedService
{
    internal class HealthCheckNotifyHostedService
        : IHostedService
    {
        private readonly ILogger<HealthCheckNotifyHostedService> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceProvider _serviceProvider;
        private readonly Settings _settings;

        private Task _executingTask;
        private CancellationTokenSource _cancellationTokenSource;

        public HealthCheckNotifyHostedService
           (IServiceProvider provider,
           IOptions<Settings> settings,
           ILogger<HealthCheckNotifyHostedService> logger,
           IHostApplicationLifetime lifetime)
        {
            _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(provider));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _settings = settings.Value ?? new Settings();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_cancellationTokenSource.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await Collect(cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // We are halting, task cancellation is expected.
                }
            });

            return Task.CompletedTask;
        }


        private async Task Collect(CancellationToken cancellationToken)
        {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Collect should not be triggered until IServerAddressFeature reports the listening endpoints

                _logger.LogDebug("Executing HealthCheck collector HostedService.");

                using (var scope = scopeFactory.CreateScope())
                {
                    try
                    {
                        //撈取UI設定
                        var db = scope.ServiceProvider.GetRequiredService<HealthChecksDb>();
                        if (db.HealthCheckUIParameter != null)
                        {
                            var time_retry = await db.HealthCheckUIParameter.FirstOrDefaultAsync();
                            _settings.NotifyEvaluationTimeInSeconds = int.Parse(time_retry.OnNotifyPushTime.ToString());
                        }

                        var runner = scope.ServiceProvider.GetRequiredService<IHealthCheckNotifyCollector>();
                        await runner.Collect(cancellationToken);

                        _logger.LogDebug("HealthCheck collector HostedService executed successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "HealthCheck collector HostedService threw an error: {Error}", ex.Message);
                    }
                }

                await Task.Delay(_settings.NotifyEvaluationTimeInSeconds * 1000, cancellationToken);
            }
        }

    }
}
